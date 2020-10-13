using System.Net.Mime;
using Bogus;
using Shipyard.Contracts.Addresses;
using Shipyard.Messages;

namespace Shipyard.Test.Fakes
{
    public sealed class EmailFaker : Faker<Email>
    {
        private static readonly EmailAddress _defaultFrom = new EmailAddress("Brian", "brian@example.com");
        private static readonly EmailAddressFaker _emailAddresses = new EmailAddressFaker();
        
        public EmailFaker()
        {
            RuleFor(e => e.From, f => _defaultFrom);
            RuleFor(e => e.To, f => _emailAddresses.Generate(f.Random.Int(1, 3)).ToArray());
            RuleFor(e => e.Cc, f => _emailAddresses.Generate(f.Random.Int(0, 2)).ToArray());
            RuleFor(e => e.Bcc, f => _emailAddresses.Generate(f.Random.Int(0, 2)).ToArray());
            RuleFor(e => e.Subject, f => f.Lorem.Sentence(5));
            RuleFor(e => e.Content, f => f.Lorem.Paragraphs());
            RuleFor(e => e.ContentType, f => new ContentType("text/plain"));
        }
    }
}
