

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shipyard.Contracts;
using Shipyard.Contracts.Commands;
using Shipyard.Contracts.MessageTypes;
using Shipyard.MessageTypes;
using Shipyard.Models;
using Shipyard.Providers;

namespace Shipyard
{
    public class EmailService
        : IEmailService
    {
        private readonly IMessageProvider<IEmail> _messageProvider;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public EmailService(IMessageProvider<IEmail> messageProvider, ISendEndpointProvider sendEndpointProvider)
        {
            _messageProvider = messageProvider;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task SendEmail(EmailModel message) 
        {            
            var emails = EmailBuilder.Expand(message); 

            var tasks = _Send(emails);            
            await Task.WhenAll(tasks);

            IEnumerable<Task> _Send(IEnumerable<IEmail> emailsToSend) 
            {
                foreach (var email in emailsToSend) 
                {
                    yield return _sendEndpointProvider.Send<ScheduleEmail>(new {
                        Email = email,
                        ScheduleTime = message.ScheduleTime
                    });
                }
            }
        }                
    }
}
