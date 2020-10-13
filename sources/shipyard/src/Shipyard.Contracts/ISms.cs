using System;
using System.Collections.Generic;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Contracts
{
    public interface ISms
    {
        MessageId Id { get; }        
        PhoneNumber From { get; }
        PhoneNumber[] To { get; }
        string Content { get; }
        Uri CallbackUrl { get; }
        Dictionary<string, string> Metadata { get;}
    }    
}
