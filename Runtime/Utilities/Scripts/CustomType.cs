using System.Collections.Generic;

namespace Virtuademy.SDK.Core.Utilities
{
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
