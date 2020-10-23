using System;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Shipyard.Bootstrap
{
    public static class MassTransitRabbitMqExtensions
    {
        private static readonly string AMPQ_SCHEME = "amqp";
        
        /// <summary>
        /// Configure the Mass Transit endpoint from a AMQP URI (https://www.rabbitmq.com/uri-spec.html).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="amqpUri"></param>
        /// <returns></returns>
        public static IRabbitMqBusFactoryConfigurator AmqpHost(this IRabbitMqBusFactoryConfigurator configurator, string amqpUri)
        {
            if (string.IsNullOrEmpty(amqpUri))
            {
                throw new ArgumentException(nameof(amqpUri));
            }
            
            var builder = new UriBuilder(amqpUri);
            
            if (string.IsNullOrEmpty(builder.Scheme) 
                || !builder.Scheme.Equals(AMPQ_SCHEME, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException(@"URI must be in the format ""amqp://user:pass@host:port/vhost""", nameof(amqpUri));
            }

            var host = string.IsNullOrEmpty(builder.Host) ? "localhost" : builder.Host;
            var port = (ushort) (builder.Port < 0 ?  5672 : builder.Port);
            var vhost = string.IsNullOrEmpty(builder.Path) ? "/" : builder.Path;
            var user = string.IsNullOrEmpty(builder.UserName) ? "guest" : builder.UserName;
            var pass = string.IsNullOrEmpty(builder.Password) ? "guest" : builder.Password;
            
            configurator.Host(host, port, vhost, c =>
            {
                c.Username(user);
                c.Password(pass);
            });

            return configurator;
        }
    }
}
