using System;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    [Serializable]
    public class SerializedType<T>
    {
        [SerializeField] string fullTypeName = typeof(T).Name;

        string cachedFullTypeName;
        Type cachedType;

        public Type Type => cachedType;

        public static implicit operator Type(SerializedType<T> t)
        {
            if (t.cachedFullTypeName != t.fullTypeName)
            {
                t.cachedFullTypeName = t.fullTypeName;
                t.cachedType = Type.GetType(t.cachedFullTypeName);
            }

            return t.cachedType;
        }

        public static implicit operator SerializedType<T>(Type t)
        {
            SerializedType<T> s = new()
            {
                fullTypeName = t.FullName
            };
            return s;
        }
    }
}
