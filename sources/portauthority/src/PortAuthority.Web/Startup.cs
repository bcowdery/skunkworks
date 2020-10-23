using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PortAuthority.Bootstrap;
using PortAuthority.Contracts;
using PortAuthority.Contracts.Commands;
using PortAuthority.Data;
using PortAuthority.Web.Extensions;
using PortAuthority.Web.Settings;

namespace PortAuthority.Web
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

            // Mvc
            services.AddCors();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
            {
                options.Conventions.UseSlugifiedRoutes();
            });
            
            // Runtime options providers
            services.AddOptions();
            services.Configure<CorsSettings>(Configuration.GetSection("CorsSettings"));
            
            // Database configuration
            services.AddDbContext<IPortAuthorityDbContext, PortAuthorityDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("SqlDatabase"),
                    providerOptions => providerOptions.EnableRetryOnFailure()));

            // MassTransit messaging endpoints
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.AmqpHost(Configuration.GetConnectionString("Rabbit"));                    
                    PortAuthorityEndpointConventions.Map();
                });
            });
            
            services.AddMassTransitHostedService();

            // Application services 
            services.AddPortAuthorityServices();

            // Open API (Swagger) documentation
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("JSON Web Token", new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                });

                var securityScheme = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "JSON Web Token",
                        Type = ReferenceType.SecurityScheme
                    },
                };

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    { securityScheme, new string[] { }},
                });

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Port Authority API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<CorsSettings> corsOptions)
        {
            // Set base path when hosting in a virtual path
            // required for load-balacing, proxies or azure front door routing
            app.UsePathBase(Configuration["PathPrefix"]);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            // Default route template is ./swagger/{documentName}/swagger.json
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint. 
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty; // serve from site root /
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "Port Authority API v1");
            });

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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
