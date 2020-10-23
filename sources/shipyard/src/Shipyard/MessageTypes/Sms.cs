using System;
using System.Collections.Generic;
using Shipyard.Contracts;
using Shipyard.Contracts.Addresses;
using Shipyard.Contracts.MessageTypes;

namespace Shipyard.MessageTypes
{
    public class Sms
        : ISms
    {
        public MessageId Id { get; set; }
        public PhoneNumber From { get; set; }
        public PhoneNumber[] To { get; set; }
        public string Content { get; set; }
        public Uri CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
