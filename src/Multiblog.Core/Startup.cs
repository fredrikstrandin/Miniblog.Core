using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Multiblog.Core.Attribute;
using Multiblog.Core.Interface.Repositories;
using Multiblog.Core.Model.Setting;
using Multiblog.Core.Repositories.User;
using Multiblog.Core.Repository;
using Multiblog.Core.Repository.Mail;
using Multiblog.Core.Repository.MongoDB;
using Multiblog.Core.Repository.VerifyUser;
using Multiblog.Core.Services;
using Multiblog.Core.Services.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Multiblog.Service;
using Multiblog.Service.UserService;
using Multiblog.Repository;
using WebEssentials.AspNetCore.OutputCaching;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;
using WilderMinds.MetaWeblog;

using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using MetaWeblogService = Multiblog.Core.Services.MetaWeblogService;
using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;
using Multiblog.Service.Blog;
using Multiblog.Service.Interface;
using Multiblog.Service.OAuth;
using Multiblog.Repository.Blog;
using Multiblog.Model.Setting;

namespace Multiblog.Core
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;

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

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(option =>
                {
                    //option.Listen(IPAddress.Loopback, 5001);
                    option.AddServerHeader = false;
                })
                .Build();

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BlogSettings>(Configuration.GetSection("blog"));
            services.Configure<UrlSettings>(Configuration.GetSection("UrlSetting"));
            services.Configure <AzureSettings>(Configuration.GetSection("AzureSetting"));
            services.Configure<MongoDbDatabaseSetting>(Configuration.GetSection("MongoDBDatabaseSetting"));

            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";

                options.Authority = Configuration["UrlSetting:OAuthServerUrl"]; ;
                options.RequireHttpsMetadata = false;

                options.ClientId = "MultiBlog";
                options.ClientSecret = "KalleIAmsterdamSniffdeLim";
                options.ResponseType = "code id_token";

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("blog");
                options.Scope.Add("offline_access");
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMetaWeblog<MetaWeblogService>();
            services.AddScoped(typeof(TenantAttribute));
            services.AddSingleton<IBlogRepository, BlogRepository>();
            
            services.AddSingleton<IOAuthServices, OAuthServices>();

            services.AddScoped<IBlogService, BlogService>();

            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IBlogPostRepository, BlogMongoDBRepository>();

            services.AddScoped<IFileRepository, FileDiskRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMailRepository, MailRepository>();

            services.AddScoped<IVerifyUserRepository, VerifyUserRepository > ();
            services.AddScoped<ITestDataService, TestDataService>();
            services.AddScoped<ITestDataRepository, TestDataRepository>();

            // Progressive Web Apps https://github.com/madskristensen/WebEssentials.AspNetCore.ServiceWorker
            services.AddProgressiveWebApp(new WebEssentials.AspNetCore.Pwa.PwaOptions
            {
                OfflineRoute = "/shared/offline/"
            });

            // Output caching (https://github.com/madskristensen/WebEssentials.AspNetCore.OutputCaching)
            services.AddOutputCaching(options =>
            {
                options.Profiles["default"] = new OutputCacheProfile
                {
                    Duration = 3600
                };
            });
            
            // HTML minification (https://github.com/Taritsyn/WebMarkupMin)
            services
                .AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    options.MinificationSettings.RemoveOptionalEndTags = false;
                    options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
                });
            services.AddSingleton<IWmmLogger, WmmNullLogger>(); // Used by HTML minifier

            // Bundling, minification and Sass transpilation (https://github.com/ligershark/WebOptimizer)
            services.AddWebOptimizer(pipeline =>
            {
                pipeline.MinifyJsFiles();
                pipeline.CompileScssFiles()
                        .InlineImages(1);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.Use((context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                if (context.Request.IsHttps)
                {
                    context.Response.Headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains";
                }
                return next();
            });

            app.UseStatusCodePagesWithReExecute("/Shared/Error");
            app.UseWebOptimizer();

            app.UseStaticFilesWithCache();

            if (Configuration.GetValue<bool>("forcessl"))
            {
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            }

            app.UseMetaWeblog("/metaweblog");
            app.UseAuthentication();

            app.UseOutputCaching();
            app.UseWebMarkupMin();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Blog}/{action=Index}/{id?}");
            });
        }
    }
}
