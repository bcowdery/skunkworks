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
            // Register services by convention
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
            services.AddScoped<ISubtaskService, SubtaskService>();
        }
    }
}
