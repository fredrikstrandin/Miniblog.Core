// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using GlobalExceptionHandler.WebApi;
using Host.Configuration;
using IdentityServer4;
using IdentityServer4.MongoDB.DependencyInjection;
using IdentityServer4.MongoDB.Model.Setting;
using IdentityServer4.MongoDB.Service;
using IdentityServer4.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Venter.Utilities.Exception;
using Miniblog.Core.Model.Setting;
using Vivus.Model.Setting;
using Vivus.Repository.DependencyInjection;
using Vivus.Services.DependencyInjection;

namespace Host
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration config,
            IHostingEnvironment environment,
            ILogger<Startup> logger)
        {
            _config = config;
            _environment = environment;
            _logger = logger;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true);
                
            if (_environment.IsEnvironment("Development"))
            {
                builder.AddUserSecrets<Startup>();
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            if (!(Configuration["AzureKeyVault:Vault"] == null || Configuration["AzureKeyVault:ClientId"] == null || Configuration["AzureKeyVault:ClientSecret"] == null))
            {
                builder.AddAzureKeyVault(
                    $"https://{Configuration["AzureKeyVault:Vault"]}.vault.azure.net/",
                    Configuration["AzureKeyVault:ClientId"],
                    Configuration["AzureKeyVault:ClientSecret"]);

                Configuration = builder.Build();
            }
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Requires using Microsoft.AspNetCore.Mvc;
            services.Configure<MvcOptions>(options => { }
            //    options =>
            //{
            //    options.Filters.Add(new RequireHttpsAttribute());
            //}
            );

            services.Configure<ApplicationSetting>(Configuration.GetSection("ApplicationSetting"));
            services.Configure<MongoDbDatabaseSetting>(Configuration.GetSection("MongoDBDatabaseSetting"));
            services.Configure<APIKeySetting>(Configuration.GetSection("APIKeySetting"));
            services.Configure<MailSetting>(Configuration.GetSection("MailSetting"));
            services.Configure<PasswordSetting>(Configuration.GetSection("PasswordSetting"));
            services.Configure<CloudStorageAccountSetting>(Configuration.GetSection("CloudStorageAccount"));

            services.AddMvc();

            services.AddScoped<IProfileService, MongoDBUserProfileService>();
            services.AddServices();
            services.AddRepository();

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            X509Certificate2 cert = null;

            if (_environment.EnvironmentName.StartsWith("Azure"))
            {
                cert = LoadAzureCertificateFromStore(Configuration["WEBSITE_LOAD_CERTIFICATES"]);
            }
            if (_environment.EnvironmentName.StartsWith("Docker"))
            {
                cert = LoadCertificateFromFile(Configuration["CertFileSetting:Filename"], Configuration["CertFileSetting:Password"]);
            }
            else
            {
                cert = LoadCertificateFromStore(Configuration["WEBSITE_LOAD_CERTIFICATES"]);
            }

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddInMemoryClients(Clients.Get())
                //.AddInMemoryClients(_config.GetSection("Clients"))
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                //.AddSigningCredential(cert)
                .AddDeveloperSigningCredential()
                .AddExtensionGrantValidator<Extensions.ExtensionGrantValidator>()
                .AddExtensionGrantValidator<Extensions.NoSubjectExtensionGrantValidator>()
                .AddJwtBearerClientAuthentication()
                .AddAppAuthRedirectUriValidator()
                .AddMongoDBUsers(Configuration);

            services.AddOidcStateDataFormatterCache("aad", "demoidsrv");

            services.AddAuthentication()
                .AddFacebook("Facebook", option =>
                {
                    option.ClientId = Configuration["FacebookSetting:AppId"];
                    option.ClientSecret = Configuration["FacebookSetting:AppSecret"];

                    option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    option.SaveTokens = true;
                })
                .AddTwitter(option =>
                {
                    option.ConsumerKey = Configuration["TwitterSetting:ConsumerKey"];
                    option.ConsumerSecret = Configuration["TwitterSetting:ConsumerSecret"];
                    option.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    option.SaveTokens = true;
                });

            return services.BuildServiceProvider(validateScopes: true);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddAzureWebAppDiagnostics();

            //app.UseDeveloperExceptionPage();
            var options = new RewriteOptions()
                //.AddRedirectToHttps()
                ;

            app.UseRewriter(options);

            app.UseExceptionHandler().WithConventions(x =>
            {
                x.ContentType = "application/json";
                x.MessageFormatter(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred whilst processing your request"
                }));

                x.ForException<UnauthorizedAccessException>()
                    .ReturnStatusCode((int)HttpStatusCode.Unauthorized)
                    .UsingMessageFormatter((ex, context) => JsonConvert.SerializeObject(new
                    {
                        Message = "Unauthorized Access"
                    }));

                x.ForException<WebMessageException>()
                    .ReturnStatusCode((int)HttpStatusCode.OK)
                    .UsingMessageFormatter((ex, context) =>
                    {
                        context.Response.StatusCode = ((WebMessageException)ex).StatusCode;

                        return JsonConvert.SerializeObject(new
                        {
                            Message = ex.Message,
                        });
                    });

                x.OnError((exception, httpContext) =>
                {
                    _logger.LogError(exception.Message);

                    TelemetryClient telemetryClient = new TelemetryClient();

                    telemetryClient.TrackException(exception);

                    return Task.CompletedTask;
                });
            });

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        public X509Certificate2 LoadCertificateFromStore(string thumbPrint)
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                List<X509Certificate2> certCollection = new List<X509Certificate2>();
                
                foreach (var item in store.Certificates)
                {
                    if (item.Thumbprint == thumbPrint)
                        certCollection.Add(item);
                }

                if (certCollection.Count == 0)
                {
                    throw new Exception("The specified certificate wasn't found.");
                }
                return certCollection[0];
            }
        }

        public X509Certificate2 LoadAzureCertificateFromStore(string thumbPrint)
        {
            X509Certificate2 cert = null;

            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                        X509FindType.FindByThumbprint,
                                        thumbPrint,
                                        false);
            // Get the first cert with the thumbprint
            if (certCollection.Count > 0)
            {
                cert = certCollection[0];
            }

            //certStore.Close();

            return cert;
        }

        public X509Certificate2 LoadCertificateFromFile(string fileName, string password)
        {
            return new X509Certificate2(fileName, password);
        }
    }
}