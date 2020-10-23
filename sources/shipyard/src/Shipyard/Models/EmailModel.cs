
using System;
using System.Collections.Generic;
using System.Net.Mime;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Models
{
    public class EmailModel
    {
        public DateTimeOffset? ScheduleTime { get; set; }

        public EmailAddress[] To { get; set; }
        public EmailAddress[] Cc { get; set; }
        public EmailAddress[] Bcc { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }

        public Uri CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public EmailPersonalizations[] Personalizations { get; set; }
    }

    public class EmailPersonalizations 
    {
        public EmailAddress[] To { get; set; }
        public EmailAddress[] Cc { get; set; }
        public EmailAddress[] Bcc { get; set; }
        public string Subject { get; set; }
        public Uri CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Substitutions { get;set; } = new Dictionary<string, string>();        
    }
}
