using System;

namespace Shipyard.Contracts.Addresses
{    
    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public EmailAddress() 
        {
        }

        public EmailAddress(string name, string address) 
        {
            Name = name;
            Address = address;
        }

        protected bool Equals(EmailAddress other)
        {
            return Name == other.Name
                && Address == other.Address;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EmailAddress) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Name, Address);

        public override string ToString() => string.IsNullOrEmpty(Name) ? Address : $"{Name} <{Address}>";
    }
}
