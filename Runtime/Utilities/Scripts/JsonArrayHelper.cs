using Newtonsoft.Json;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class JsonArrayHelper
    {
        public static T[] FromJson<T>(string json) => JsonConvert.DeserializeObject<T[]>(json);

        public static string ToArrayJson<T>(T[] array) => JsonConvert.SerializeObject(array);

        public static string ToArrayJson<T>(T[] array, bool prettyPrint) => JsonConvert.SerializeObject(array, Formatting.Indented);
    }
}