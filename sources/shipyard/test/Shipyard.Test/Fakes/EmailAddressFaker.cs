using Bogus;
using Shipyard.Contracts.Addresses;

namespace Shipyard.Test.Fakes
{
    public sealed class EmailAddressFaker : Faker<EmailAddress>
    {
        public EmailAddressFaker()
        {
            RuleFor(a => a.Name, f => f.Name.FullName());
            RuleFor(a => a.Address, f => f.Internet.Email());
        }
    }
}
