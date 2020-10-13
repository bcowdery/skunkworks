using System;

namespace Shipyard.Contracts.Addresses
{    
    public class PhoneNumber
    {        
        public string Number { get; set; }      


        protected bool Equals(PhoneNumber other)
        {
            return Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PhoneNumber) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Number);

        public override string ToString() => Number;
    }
}
