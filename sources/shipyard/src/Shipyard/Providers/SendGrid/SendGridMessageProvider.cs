
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Shipyard.Contracts;

namespace Shipyard.Providers.SendGrid
{
    public class SendGridMessageProvider 
        : IMessageProvider<IEmail>
    {
        private readonly IOptions<SendGridSettings> _sendGridOptions;

        public SendGridMessageProvider(IOptions<SendGridSettings> sendGridOptions)
        {
            _sendGridOptions = sendGridOptions;
        }

        protected SendGridSettings SendGridSettings => _sendGridOptions.Value;

        public Task<ISendResponse> SendAsync(IEmail message)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SendGridSendResult
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
