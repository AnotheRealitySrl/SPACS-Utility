using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Static utility functions for Reflection operations
    /// </summary>
    public static class ReflectionUtilities
    {
        public const BindingFlags BindingInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Returns all the fields (public and not-public) of a specific
        /// type</summary>
        /// <param name="type">The type to get the fields from</param>
        /// <returns>The fields</returns>
        public static IEnumerable<FieldInfo> GetInstanceFields(Type type)
        {
            return type.GetFields(BindingInstanceFlags);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Returns all the SerializeField of a specific type</summary>
        /// <param name="type">The type to get the fields from</param>
        /// <returns>The serialized fields</returns>
        public static IEnumerable<FieldInfo> GetSerializedFields(Type type)
        {
            return GetInstanceFields(type).Where(f => f.GetCustomAttribute<SerializeField>() != null);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>Returns all the type (specified in any assembly) that inherits
        /// from the specified base types</summary>
        /// <param name="baseTypes">The base types used to search</param>
        /// <returns>The types that inherit from the specified types</returns>
        public static IEnumerable<Type> GetAssignableClasses(params Type[] baseTypes)
        {
            if (baseTypes == null || baseTypes.Length == 0)
                return null;

            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseTypes.All(b => t != b && b.IsAssignableFrom(t)));
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a field setter for a specified type. This allows to have a
        /// fast and reliable method to change a not-accessible field.
        /// </summary>
        /// <typeparam name="T">The type of the class that contains the field</typeparam>
        /// <param name="fieldName">The exact field name</param>
        /// <returns>An action that can be invoked as a setter</returns>
        public static Action<T, object> GenerateFieldSetter<T>(string fieldName)
        {
            FieldInfo fieldInfo = typeof(T).GetField(fieldName, BindingInstanceFlags);
            return GenerateSetter<T>(fieldInfo);
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a member setter for a specified type. This allows to have a
        /// fast and reliable method to change a not-accessible member.
        /// </summary>
        /// <typeparam name="T">The type of the class that contains the member</typeparam>
        /// <param name="memberInfo">The member info</param>
        /// <returns>An action that can be invoked as a setter</returns>
        public static Action<T, object> GenerateSetter<T>(MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            var exInstance = Expression.Parameter(targetType, "t");

            var exMemberAccess = Expression.MakeMemberAccess(exInstance, memberInfo);

            var exValue = Expression.Parameter(typeof(object), "p");
            var exConvertedValue = Expression.Convert(exValue, GetUnderlyingType(memberInfo));
            var exBody = Expression.Assign(exMemberAccess, exConvertedValue);

            var lambda = Expression.Lambda<Action<T, object>>(exBody, exInstance, exValue);
            var action = lambda.Compile();
            return action;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Generates a member getter for a specified type. This allows to have a
        /// fast and reliable method to retrieve a not-accessible member value.
        /// </summary>
        /// <typeparam name="T">The type of the class that contains the member</typeparam>
        /// <param name="memberInfo">The member info</param>
        /// <returns>A function that can be invoked as a getter</returns>
        public static Func<T, object> GenerateGetter<T>(MemberInfo memberInfo)
        {
            var targetType = memberInfo.DeclaringType;
            var exInstance = Expression.Parameter(targetType, "t");

            var exMemberAccess = Expression.MakeMemberAccess(exInstance, memberInfo);
            var exConvertToObject = Expression.Convert(exMemberAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(exConvertToObject, exInstance);

            var action = lambda.Compile();
            return action;
        }

        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns the underlying type of a member (event, field, method or property)
        /// </summary>
        /// <param name="member">The member info</param>
        /// <returns>The underlying type of the member</returns>
        public static Type GetUnderlyingType(MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.Event => ((EventInfo)member).EventHandlerType,
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Method => ((MethodInfo)member).ReturnType,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                _ => throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"),
            };
        }

        public static IEnumerable<FieldInfo> GetFieldsRecurse<BaseType>(BaseType obj, BindingFlags flags) where BaseType : class
        {
            var type = obj.GetType();
            while (type != typeof(BaseType))
            {
                foreach (var field in type.GetFields(flags | BindingFlags.DeclaredOnly))
                    yield return field;
                type = type.BaseType;
            }
        }

        public static IEnumerable<PropertyInfo> GetPropertiesRecurse<BaseType>(BaseType obj, BindingFlags flags) where BaseType : class
        {
            var type = obj.GetType();
            while (type != typeof(BaseType))
            {
                foreach (var property in type.GetProperties(flags | BindingFlags.DeclaredOnly))
                    yield return property;
                type = type.BaseType;
            }
        }

        public static MethodInfo GetMethodRecurse<BaseType>(BaseType obj, string name, BindingFlags flags) where BaseType : class
        {
            var type = obj.GetType();
            MethodInfo result = null;
            while (type != typeof(BaseType) && result == null)
            {
                result = type.GetMethod(name, flags | BindingFlags.DeclaredOnly);
                type = type.BaseType;
            }
            return result;
        }
    }
}