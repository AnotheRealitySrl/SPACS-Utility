using System.Collections.Generic;

namespace SPACS.Utilities
{
    /// <remarks>
    /// The converter is named on the type so that it applies whether or not the application
    /// installed <see cref="JsonConverters.SetJsonConvertDefaultSettings"/>: without it, an app
    /// that had not (an external one, which has no AppManager to do it) read every saved value
    /// into an empty object, since <see cref="Fields"/> has no setter.
    /// </remarks>
    [Newtonsoft.Json.JsonConverter(typeof(CustomTypeConverter))]
    public class CustomType
    {
        private Dictionary<string, object> _fields = new Dictionary<string, object>();

        public Dictionary<string, object> Fields { get => _fields; }

        public object GetValue(string key)
        {
            if (Fields.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
