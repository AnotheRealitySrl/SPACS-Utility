using System;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class TypeExtensions
    {
        public static string LastPartOfTypeName(this Type type)
        {
            string[] typeNameFull = type.FullName?.Split('.');
            string typeNameLast = typeNameFull?[typeNameFull.Length - 1];
            return typeNameLast;
        }

        public static Type GetTextType(this TextAsset text)
        {
            return text.GetTextClassCompleteName().GetTypeFromString();
        }

        public static Type GetTypeFromString(this string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.FullName == typeName)
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}
