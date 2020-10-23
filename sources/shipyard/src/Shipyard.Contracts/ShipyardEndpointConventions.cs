using System;
using MassTransit;

namespace Shipyard.Contracts
{
    /// <summary>
    /// Endpoint conventions and queue names for PortAuthority message contracts.
    /// </summary>
    public static class ShipyardEndpointConventions
    {
        public const string ScheduleEndpoint = "shipyard-schedule";
        public const string EmailEndpoint = "shipyard-email";
        public const string SmsEndpoint = "shipyard-sms";
        
        /// <summary>
        /// Maps all PortAuthority message types.
        /// </summary>
        public static void Map()
        {
            // todo.
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
