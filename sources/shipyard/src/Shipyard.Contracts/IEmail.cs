
using System;
using System.Collections.Generic;
using System.Net.Mime;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Contracts
{
    public interface IEmail
    {        
        MessageId Id { get; }

        EmailAddress From { get; }
        EmailAddress[] To { get; }
        EmailAddress[] Cc { get; }
        EmailAddress[] Bcc { get; }

        string Subject { get; }
        string Content { get; }
        ContentType ContentType { get; }

        Uri CallbackUrl { get; }
        Dictionary<string, string> Metadata { get; }
    }
}
