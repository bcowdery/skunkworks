using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Shipyard.Contracts
{
    public readonly struct MessageId
    {   
        private static readonly char[] Base64Padding = { '=' };

        private readonly string _value;

        public MessageId(string value) 
        {            
            _value = value;
        }

        public string Value => _value;

        public static MessageId CreateNew(params object[] args) 
        {
            var plainText = String.Join(":", args.FlattenToStrings().ToArray());
            var hash = CalculateHash(plainText);

            return new MessageId(hash);
        }

        private static string CalculateHash(string value) 
        {
            if (string.IsNullOrWhiteSpace(value)) 
                return string.Empty;
            
            var bytes = Encoding.UTF8.GetBytes(value);

            using (var sha1 = SHA1.Create()) 
            {
                var hash = sha1.ComputeHash(bytes);
                return Convert.ToBase64String(hash)
                    .TrimEnd(Base64Padding)
                    .Replace('+', '-')
                    .Replace('/', '_');
            }
        }

        public static implicit operator string(MessageId id) => id._value;
        public static explicit operator MessageId(string id) => new MessageId(id);

        public override string ToString() => _value;
    }

    internal static class ArrayExtensions
    {
        public static IEnumerable<string> FlattenToStrings(this object[] values)
        {
            foreach (var value in values)
            {
                switch (value)
                {
                    case string s:
                        yield return s;
                        break;
                    
                    case IEnumerable items:
                    {
                        foreach (var item in items)
                        {
                            yield return Convert.ToString(item);
                        }

                        break;
                    }
                    
                    default:
                        yield return Convert.ToString(value);
                        break;
                }
            }
        }
    }    
}
