
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shipyard.Contracts;

namespace Shipyard.Providers.Twilio
{
    public class TwilioMessageProvider 
        : IMessageProvider<ISms>
    {
        private readonly IOptions<TwilioSettings> _twilioOptions;

        public TwilioMessageProvider(IOptions<TwilioSettings> twilioOptions)
        {
            _twilioOptions = twilioOptions;
        }

        protected TwilioSettings TwilioSettings => _twilioOptions.Value;

        public Task<ISendResponse> SendAsync(ISms message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TwilioSendResponse 
        : ISendResponse
    {
        public string MessageId => throw new NotImplementedException();

        public DateTimeOffset? SentDate => throw new NotImplementedException();

        public string StatusMessage => throw new NotImplementedException();

        public HttpStatusCode StatusCode => throw new NotImplementedException();

        public bool IsError()
        {
            throw new NotImplementedException();
        }

        public bool IsQueued()
        {
            throw new NotImplementedException();
        }

        public bool IsSent()
        {
            throw new NotImplementedException();
        }
    }
}
