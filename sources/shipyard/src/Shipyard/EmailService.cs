

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Shipyard.Contracts;
using Shipyard.Contracts.Commands;
using Shipyard.Messages;
using Shipyard.Models;
using Shipyard.Providers;

namespace Shipyard
{
    public class EmailService
    {
        private readonly IMessageProvider<IEmail> _messageProvider;
        private readonly ISendEndpoint _sendEndpoint;

        public EmailService(IMessageProvider<IEmail> messageProvider, ISendEndpoint sendEndpoint)
        {
            _messageProvider = messageProvider;
            _sendEndpoint = sendEndpoint;
        }

        public async Task SendEmail(EmailModel message) 
        {            
            var emails = EmailBuilder.Expand(message); 

            var tasks = _Send(emails);            
            await Task.WhenAll(tasks);

            IEnumerable<Task> _Send(IEnumerable<IEmail> emails) 
            {
                foreach (var email in emails) 
                {
                    yield return _sendEndpoint.Send<ScheduleEmail>(new {
                        Email = email,
                        Schedule = message.Schedule
                    });
                }
            }
        }                
    }
}
