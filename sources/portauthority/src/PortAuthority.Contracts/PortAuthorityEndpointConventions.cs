using System;
using MassTransit;
using PortAuthority.Contracts.Commands;

namespace PortAuthority.Contracts
{
    /// <summary>
    /// Endpoint conventions and queue names for PortAuthority message contracts.
    /// </summary>
    public static class PortAuthorityEndpointConventions
    {
        public const string JobEndpoint = "port-authority-jobs";
        public const string SubtaskEndpoint = "port-authority-tasks";
        
        /// <summary>
        /// Maps all PortAuthority message types.
        /// </summary>
        public static void Map()
        {
            // job commands
            Map<CreateJob>(new Uri($"queue:{JobEndpoint}"));
            Map<StartJob>(new Uri($"queue:{JobEndpoint}"));
            Map<EndJob>(new Uri($"queue:{JobEndpoint}"));
            
            // task commands
            Map<CreateSubtask>(new Uri($"queue:{SubtaskEndpoint}"));
            Map<StartSubtask>(new Uri($"queue:{SubtaskEndpoint}"));
            Map<EndSubtask>(new Uri($"queue:{SubtaskEndpoint}"));            
        }
        
        /// <summary>
        /// Maps an endpoint convention if not already registered.
        /// </summary>
        /// <param name="uri"></param>
        /// <typeparam name="T"></typeparam>
        private static void Map<T>(Uri uri)
            where T : class
        {
            if (!EndpointConvention.TryGetDestinationAddress<T>(out _))
                EndpointConvention.Map<T>(uri);
        }
    }
}
