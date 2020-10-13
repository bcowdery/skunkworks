using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shipyard.Providers
 {     
    public interface IMessageProvider<in TMessage>
    {        
        Task<ISendResponse> SendAsync(TMessage message);        
    }

    public interface ISendResponse
    {        
        string MessageId { get; }        
        DateTimeOffset? SentDate { get; } 

        string StatusMessage { get; }
        HttpStatusCode StatusCode { get; }
        
        bool IsSent();
        bool IsQueued();
        bool IsError();               
    }
}
