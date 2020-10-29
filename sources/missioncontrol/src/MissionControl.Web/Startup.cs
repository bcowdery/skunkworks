using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MissionControl.Web.Settings;
using PortAuthority.Web.Extensions;

namespace MissionControl.Web
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
            // Application Insights
            services.AddApplicationInsightsTelemetry();
            
            // Healthchecks UI
            services.AddHealthChecks();
            services.AddHealthChecksUI()
                .AddSqlServerStorage(Configuration.GetConnectionString("SqlDatabase"));

            // Mvc            
            services.AddCors();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddControllersWithViews(options =>
            {
                options.Conventions.UseSlugifiedRoutes();
            });
                        
            // Runtime options providers
            services.AddOptions();
            services.Configure<CorsSettings>(Configuration.GetSection("CorsSettings"));    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<CorsSettings> corsOptions)
        {
            // Set base path when hosting in a virtual path
            // required for load-balancing, proxies or azure front door routing
            app.UsePathBase(Configuration["PathPrefix"]);
            
            // Developer error pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // HTTP Request pipeline
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Global CORS policy
            var corsSettings = corsOptions.Value;
            var allowedOrigins = corsSettings.GetAllowedOrigins();

            app.UseCors(x => x
                .WithOrigins(allowedOrigins)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader());
            
            // Support for proxy load balancers and request forwarding
            // @see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.All,
                KnownNetworks = { },
                KnownProxies = { },
                AllowedHosts = Configuration["AllowedHosts"].Split(',').ToList()
            });

            app.UseCertificateForwarding();

            // Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Controller endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecksUI(options => {
                    options.UIPath = "/health";
                    options.ApiPath = $"{Configuration["PathPrefix"]}/healthcheck-api";
                    options.UseRelativeApiPath = true;
                });
                
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
