using System;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Providers.Twilio
{
    public class TwilioSettings
    {
        public string Sid { get; set; }
        public string SidToken { get; set; }
        public PhoneNumber[] PhoneNumbers { get; set; }         
        public Uri CallbackUrl { get; set; }
    }
}
