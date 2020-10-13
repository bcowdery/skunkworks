
using System;
using System.Collections.Generic;
using System.Net.Mime;
using Shipyard.Contracts;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Messages
{
    public class Email
        : IEmail
    {
        public MessageId Id { get; set; }

        public EmailAddress From { get; set; }
        public EmailAddress[] To { get; set; }
        public EmailAddress[] Cc { get; set; }
        public EmailAddress[] Bcc { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }

        public Uri CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
