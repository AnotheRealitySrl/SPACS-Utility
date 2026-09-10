using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class JsonConverters
    {
        public static void SetJsonConvertDefaultSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new JsonConverter[]
                {
                    new Vector3Converter(),
                    new QuaternionConverter(),
                    new ColorConverter(),
                    new StringEnumConverter(),
                    new CustomTypeConverter(),
                }
            };
        }
    }

    /// <summary>
    /// This class provides a JsonConverter class to convert a Unity Vector3
    /// </summary>
    public class Vector3Converter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Vector3 v)
            {
                serializer.Serialize(writer, v.SerializeVector3());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return (reader.Value as string).DeserializeVector3();
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3) || objectType == typeof(Vector3?);
        }
    }

    /// <summary>
    /// This class provides a JsonConverter class to convert a Unity Quaternion
    /// </summary>
    public class QuaternionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Quaternion q)
            {
                serializer.Serialize(writer, q.eulerAngles.SerializeVector3());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                if ((reader.Value as string).DeserializeVector3() is Vector3 euler)
                {
                    return Quaternion.Euler(euler);
                }
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion) || objectType == typeof(Quaternion?);
        }
    }

    /// <summary>
    /// This class provides a JsonConverter class to convert a Flags enum to/from an array of names.
    /// For example: ["Teacher", "Student"] is converted to Role.Teacher | Role.Student
    /// </summary>
    public class EnumFlagsConverter<T> : JsonConverter where T : Enum
    {
        readonly Dictionary<string, T> namesAndValues;

        public EnumFlagsConverter()
        {
            var names = typeof(T).GetEnumNames();
            var values = typeof(T).GetEnumValues();
            namesAndValues = Enumerable.Range(0, names.Length).ToDictionary(i => names[i], i => (T)values.GetValue(i));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value is T e)
            {
                foreach (var (n, v) in namesAndValues)
                {
                    if (e.HasFlag(v))
                    {
                        writer.WriteValue(n);
                    }
                }
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                int result = 0;
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    if (namesAndValues.TryGetValue(reader.Value as string, out T value))
                    {
                        result |= Convert.ToInt32(value);
                    }
                }
                return (T)(object)result; // this double cast sucks, thanks C#
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }

    /// <summary>
    /// This class provides a JsonConverter class to convert a Unity Color
    /// </summary>
    public class ColorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Color c)
            {
                int r = Mathf.RoundToInt(c.r * 255.0f);
                int g = Mathf.RoundToInt(c.g * 255.0f);
                int b = Mathf.RoundToInt(c.b * 255.0f);
                int a = Mathf.RoundToInt(c.a * 255.0f);
                serializer.Serialize(writer, $"#{r:X2}{g:X2}{b:X2}{a:X2}");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string value = reader.Value as string;
                const float factor = 1.0f / 255.0f;
                try
                {
                    float r = Convert.ToInt32(value[1..3], 16) * factor;
                    float g = Convert.ToInt32(value[3..5], 16) * factor;
                    float b = Convert.ToInt32(value[5..7], 16) * factor;
                    float a = Convert.ToInt32(value[7..9], 16) * factor;
                    return new Color(r, g, b, a);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color) || objectType == typeof(Color?);
        }
    }

    public class CustomTypeConverter : JsonConverter<CustomType>
    {
        public override void WriteJson(JsonWriter writer, CustomType value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (var field in value.Fields)
            {
                writer.WritePropertyName(field.Key);
                serializer.Serialize(writer, field.Value);
            }
            writer.WriteEndObject();
        }

        public override CustomType ReadJson(JsonReader reader, Type objectType, CustomType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var customType = new CustomType();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {

                    string key = reader.Value.ToString();

                    reader.Read();
                    object value = reader.Value;

                    customType.Fields[key] = value;
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }
            return customType;
        }
    }
}