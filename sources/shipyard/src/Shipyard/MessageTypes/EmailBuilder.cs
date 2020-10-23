using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Shipyard.Contracts;
using Shipyard.Contracts.Addresses;
using Shipyard.Contracts.MessageTypes;
using Shipyard.Models;

namespace Shipyard.MessageTypes
{
    public class EmailBuilder
    {
        private EmailAddress _from;
        private HashSet<EmailAddress> _to = new HashSet<EmailAddress>();
        private HashSet<EmailAddress> _cc = new HashSet<EmailAddress>();
        private HashSet<EmailAddress> _bcc = new HashSet<EmailAddress>();
        private string _originalSubject;
        private string _originalContent;
        private string _subject;    
        private string _content;
        private ContentType _contentType;
        private Uri _callbackUrl;
        private Dictionary<string, string> _metadata;

        private EmailBuilder() 
        {
        }

        public static EmailBuilder CreateNew() 
        {
            return new EmailBuilder();
        }

        public static IEnumerable<IEmail> Expand(EmailModel message, Action<EmailBuilder> config = null) 
        {
            var builder = CreateNew()
                .AddTo(message.To)
                .AddCc(message.Cc)
                .AddBcc(message.Bcc)
                .SetSubject(message.Subject)
                .SetContent(message.Content, message.ContentType)
                .SetMetadata(message.Metadata);
                
            // Single message, build and return with no personalization
            if (message.Personalizations == null && message.Personalizations.Length == 0) 
            {                
                config?.Invoke(builder);
                yield return builder.Build(); 
            }

            // Personalized message, allow personalization to override any value
            // Apply text repalcement substitutions to the message content
            foreach (var personalization in message.Personalizations) 
            {                
                if (personalization.To.Length > 0) builder.SetTo(personalization.To);
                if (personalization.Cc.Length > 0) builder.SetCc(personalization.Cc);
                if (personalization.Bcc.Length > 0) builder.SetBcc(personalization.Bcc);                
                if (personalization.CallbackUrl != null) builder.SetCallbackUrl(personalization.CallbackUrl);
                if (personalization.Metadata != null) builder.SetMetadata(personalization.Metadata);
                
                builder.ApplySubstitutions(personalization.Substitutions);

                config?.Invoke(builder);
                yield return builder.Build();
            }                                   
        }
        
        public EmailBuilder SetFrom(EmailAddress from) 
        {
            _from = from;
            return this;
        }

        public EmailBuilder SetTo(params EmailAddress[] addresses) 
        {           
            _to = addresses.ToHashSet();
            return this;
        }

        public EmailBuilder AddTo(params EmailAddress[] addresses) 
        {           
            foreach (var to in addresses) 
            {
                _to.Add(to);
            }

            return this;
        }

        public EmailBuilder AddTo(string name, string address) 
        {           
            _to.Add(new EmailAddress(name, address));

            return this;
        }

        public EmailBuilder SetCc(params EmailAddress[] addresses) 
        {           
            _cc = addresses.ToHashSet();
            return this;
        }

        public EmailBuilder AddCc(params EmailAddress[] addresses) 
        {
            foreach (var cc in addresses) 
            {
                _cc.Add(cc);
            }
            
            return this;
        }        

        public EmailBuilder AddCc(string name, string address) 
        {           
            _cc.Add(new EmailAddress(name, address));

            return this;
        }

        public EmailBuilder SetBcc(params EmailAddress[] addresses) 
        {           
            _bcc = addresses.ToHashSet();
            return this;
        }

        public EmailBuilder AddBcc(params EmailAddress[] addresses) 
        {
            foreach (var bcc in addresses) 
            {
                _bcc.Add(bcc);
            }
            
            return this;
        }              

        public EmailBuilder AddBcc(string name, string address) 
        {           
            _bcc.Add(new EmailAddress(name, address));

            return this;
        }

        public EmailBuilder SetSubject(string subject) 
        {
            _subject = subject;
            _originalSubject = subject;
            return this;
        }               
        
        public EmailBuilder SetContent(string content, ContentType contentType = null) 
        {
            _content = content;
            _originalContent = content;
            _contentType = contentType ?? new ContentType("text/plain");
            return this;
        }               

        public EmailBuilder SetCallbackUrl(Uri callbackUrl) 
        {
            _callbackUrl = callbackUrl;
            return this;
        }

        public EmailBuilder SetMetadata(Dictionary<string, string> metadata) 
        {
            _metadata = metadata;
            return this;
        }

        public EmailBuilder ApplySubstitutions(Dictionary<string, string> substitutions) 
        {
            _subject = _originalSubject;
            _content = _originalContent;
            
            foreach (var substitution in substitutions) 
            {                
                _subject = _subject.Replace(substitution.Key, substitution.Value);
                _content = _content.Replace(substitution.Key, substitution.Value);
            }

            return this;
        }

        public IEmail Build() 
        {
            return new Email() 
            {
                Id = MessageId.CreateNew(_to, _cc, _bcc, _subject, _content),                
                From = _from,
                To = _to.ToArray(),
                Cc = _cc.ToArray(),
                Bcc = _bcc.ToArray(),
                Subject = _subject,
                Content = _content,
                ContentType = _contentType,
                CallbackUrl = _callbackUrl,
                Metadata = _metadata
            };
        }        
    }
}
