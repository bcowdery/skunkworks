using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace PortAuthority.Data.Json
{
    public static class JsonDictionary
    {
        public static ValueComparer ValueComparer = new ValueComparer<Dictionary<string, object>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))
        );

        public static ValueConverter ValueConverter = new ValueConverter<Dictionary<string, object>, string>(
            v => JsonConvert.SerializeObject(v),
            v => v == null
                ? new Dictionary<string, object>()
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(v)
        );
    }
}
