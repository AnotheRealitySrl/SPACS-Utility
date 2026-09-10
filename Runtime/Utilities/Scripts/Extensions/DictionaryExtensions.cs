using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class DictionaryExtensions
    {
        public static string CustomToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(", ", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }

        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }
    }
}
