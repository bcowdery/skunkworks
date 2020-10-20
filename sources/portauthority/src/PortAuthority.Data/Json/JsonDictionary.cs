using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace PortAuthority.Data.Json
{
    /// <summary>
    /// Helpers for persisting a <see cref="Dictionary{TKey,TValue}"/> collection to JSON. 
    /// </summary>
    public static class JsonDictionary
    {
        /// <summary>
        /// Compares <see cref="Dictionary{TKey,TValue}"/> collections to the EF snapshot value to ensure
        /// that it is persisted correctly when using the JSON <see cref="ValueConverter"/>. 
        /// </summary>
        public static ValueComparer ValueComparer = new ValueComparer<Dictionary<string, object>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))
        );

        /// <summary>
        /// Converts <see cref="Dictionary{TKey,TValue}"/> collections to and from a JSON string persisted in the database.
        /// </summary>
        public static ValueConverter ValueConverter = new ValueConverter<Dictionary<string, object>, string>(
            v => JsonConvert.SerializeObject(v),
            v => v == null
                ? new Dictionary<string, object>()
                : JsonConvert.DeserializeObject<Dictionary<string, object>>(v)
        );
    }
}
