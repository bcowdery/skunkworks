using System;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Providers.SendGrid
{
    public class SendGridSettings
    {
        public string ApiKey { get; set; }
        public EmailAddress BccAddress { get; set; } 
        public EmailAddress FromAddress { get; set; }
        public Uri CallbackUrl { get; set; }
    }
}
