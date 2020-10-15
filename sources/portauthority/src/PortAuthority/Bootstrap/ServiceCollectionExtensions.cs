using System;
using System.Linq;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortAuthority.Assemblers;
using PortAuthority.Data;
using PortAuthority.Data.Entities;
using PortAuthority.Models;
using Scrutor;

namespace PortAuthority.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register required services for the Port Authority system.
        /// </summary>
        /// <param name="services"></param>
        public static void AddPortAuthorityServices(this IServiceCollection services) 
        {
            services.ValidateRequired<IPortAuthorityDbContext, PortAuthorityDbContext>();
            services.ValidateRequired<DbContextOptions<PortAuthorityDbContext>>();
          
            // Dummies
            // TODO: Placeholder for MassTransit - good enough for debugging & TDD!
            services.AddTransient<ISendEndpoint, DummySendEndpoint>();
            
            // Conventions
            services.Scan(scan => scan
                .FromAssemblies(AssemblyHook.Assembly)

                // Assemblers
                .AddClasses(c => c.AssignableTo(typeof(IAssembler<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()

                // Validators
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );
            
            // Declared services            
            services.AddScoped<IJobService, JobService>();
        }

        /// <summary>
        /// Validates that a required service registration exists.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <exception cref="ApplicationException">Thrown if the required service has not been registered</exception>
        private static void ValidateRequired<TService>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var serviceType = typeof(TService);
            var serviceRegistration = services
                .Where(x => x.Lifetime == lifetime)
                .FirstOrDefault(x => x.ServiceType == serviceType);

            if (serviceRegistration == null)
            {
                throw new ApplicationException($"Required service <{serviceType}> has not been registered!");
            }
        }

        /// <summary>
        /// Validates that a service registration exists for the given type.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="services"></param>
        /// <param name="lifetime"></param>
        /// <exception cref="ApplicationException">Thrown if the required service has not been registered</exception>
        private static void ValidateRequired<TService, TImpl>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var serviceType = typeof(TService);
            var serviceImplType = typeof(TImpl);
            var serviceRegistration = services
                .Where(x => x.Lifetime == lifetime)
                .FirstOrDefault(x => x.ServiceType == serviceType && x.ImplementationType == serviceImplType);

            if (serviceRegistration == null)
            {
                throw new ApplicationException($"Required service <{serviceType}, {serviceImplType}> has not been registered!");
            }
        }
    }
}
