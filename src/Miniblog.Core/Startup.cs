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
using Miniblog.Core.Attribute;
using Miniblog.Core.Repository;
using Miniblog.Core.Repository.MongoDB;
using Miniblog.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using WebEssentials.AspNetCore.OutputCaching;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;
using WilderMinds.MetaWeblog;

using IWmmLogger = WebMarkupMin.Core.Loggers.ILogger;
using MetaWeblogService = Miniblog.Core.Services.MetaWeblogService;
using WmmNullLogger = WebMarkupMin.Core.Loggers.NullLogger;

namespace Miniblog.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "MultiBlog";
                options.ClientSecret = "KalleIAmsterdamSniffdeLim";
                options.ResponseType = "code id_token";

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("blog");
                options.Scope.Add("offline_access");
            });

            services.AddSingleton<IUserServices, BlogUserServices>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMetaWeblog<MetaWeblogService>();
            services.AddScoped(typeof(TenantAttribute));
            //services.AddSingleton<IBlogService, FileBlogService>();
            //services.AddSingleton<IBlogRepository, BlogXmlRepository>();
            //services.AddSingleton<IFileRepository, FileDiskRepository>();

            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogRepository, BlogMongoDBRepository>();
            services.AddScoped<IFileRepository, FileDiskRepository>();

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

            //// Cookie authentication.
            //services
            //    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //    {
            //        options.LoginPath = "/login/";
            //        options.LogoutPath = "/logout/";
            //    });

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
