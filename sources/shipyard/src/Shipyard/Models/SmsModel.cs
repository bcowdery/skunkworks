
using System;
using System.Collections.Generic;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Models
{
    public class SmsModel
    {                
        public DateTimeOffset? ScheduleTime { get; set; }
        
        public PhoneNumber[] To { get; set; }
        
        public string Content { get; set; }
        
        public Uri CallbackUrl { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public SmsPersonalization[] Personalizations { get; set; }
    }    

    public class SmsPersonalization 
    {        
        public PhoneNumber[] To { get; set; }
        public Uri CallbackUrl { get; set; }        
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Substitutions { get;set; } = new Dictionary<string, string>();
    }
}
