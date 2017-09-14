using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImageGallery.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ImageGallery.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = 
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //})
            //.AddCookie(options =>
            //    {
            //        options.LoginPath = "/Account/Login";
            //        options.LogoutPath = "/Account/Logoff";
            //    })
            //.AddOpenIdConnect(options =>
            //{
            //    options.Authority = "https://localhost:44353/";
            //    options.RequireHttpsMetadata = true;
            //    options.ClientId = "imagegalleryclient";
            //    options.ClientSecret = "secret";
            //    //options.Scope.Add("openid");
            //    //options.Scope.Add("profile");
            //    //options.ResponseType = "code id_token";
            //    //options.SignInScheme = "Cookies";
            //    //options.SaveTokens = true;
            //});

            //services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores();
            //services.ConfigureApplicationCookie(options => options.LoginPath = "/Account/LogIn");

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logoff";
                })
                .AddOpenIdConnect(options =>
                {
                    //options.Authority = Configuration["auth:oidc:authority"];
                    //options.ClientId = Configuration["auth:oidc:clientid"];

                    options.Authority = "https://localhost:44353/";
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "imagegalleryclient";
                    options.ClientSecret = "secret";
                    //options.Scope.Add("openid");
                    //options.Scope.Add("profile");
                    //options.ResponseType = "code id_token";
                    //options.CallbackPath = new PathString("...");
                    //options.SignInScheme = "Cookies";
                    //options.SaveTokens = true;
                });

            services.AddMvc();

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }
    }
}
