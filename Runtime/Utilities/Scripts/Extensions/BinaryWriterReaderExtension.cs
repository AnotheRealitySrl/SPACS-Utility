using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class BinaryWriterReaderExtension
    {
        #region Int32 Array

        public static void Write(this BinaryWriter binaryWriter, int[] values)
        {
            binaryWriter.Write(values.Length);
            foreach (var value in values)
            {
                binaryWriter.Write(value);
            }
        }

        public static int[] ReadInt32s(this BinaryReader binaryReader)
        {
            int length = binaryReader.ReadInt32();

            int[] array = new int[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = binaryReader.ReadInt32();
            }

            return array;
        }
        #endregion

        #region Vector2
        public static void Write(this BinaryWriter binaryWriter, Vector2 vector2)
        {
            binaryWriter.Write(vector2.x);
            binaryWriter.Write(vector2.y);
        }

        public static Vector2 ReadVector2(this BinaryReader binaryReader)
        {
            return new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }
        #endregion

        #region Vector3
        public static void Write(this BinaryWriter binaryWriter, Vector3 vector3)
        {
            binaryWriter.Write(vector3.x);
            binaryWriter.Write(vector3.y);
            binaryWriter.Write(vector3.z);
        }

        public static Vector3 ReadVector3(this BinaryReader binaryReader)
        {
            return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }
        #endregion

        public enum SupportedType
        {
            Bool = 0,
            Byte = 1,
            SByte = 2,
            Char = 3,
            Short = 4,
            UShort = 5,
            Int = 6,
            UInt = 7,
            Long = 8,
            ULong = 9,
            Float = 10,
            Double = 11,
            Decimal = 12,
            String = 13,
            Vector2 = 14,
            Vector3 = 15
            // the current implementation only allows for 16 types since we are using 1 byte to encode 
            // 2 object types for bandwidth reasons
        }

        public static void Write(this BinaryWriter writer, object[] values)
        {
            writer.Write(values.Length);
            writer.WriteTypes(values);
            foreach (var value in values)
            {
                writer.Write(value);
            }
        }



        public static void WriteTypes(this BinaryWriter writer, object[] values)
        {
            for (int i = 0; i < values.Length; i += 2)
            {
                // Get the next 2 types
                SupportedType type1 = GetSupportedType(values[i]);
                // Type 2 is present only if we do not exceed the array count
                SupportedType type2 = (i + 1 < values.Length) ? GetSupportedType(values[i + 1]) : SupportedType.Bool;

                WriteTypes(writer, type1, type2);
            }
        }

        public static void WriteTypes(this BinaryWriter writer, SupportedType type1, SupportedType type2)
        {
            // Pack the two SupportedType values into a single byte
            byte packedByte = 0;
            packedByte |= (byte)type1;
            packedByte |= (byte)((byte)type2 << 4); // Insert type2 in the last 4 bit

            writer.Write(packedByte);
        }

        private static SupportedType GetSupportedType(object obj)
        {
            return obj switch
            {
                bool => SupportedType.Bool,
                byte => SupportedType.Byte,
                sbyte => SupportedType.SByte,
                char => SupportedType.Char,
                short => SupportedType.Short,
                ushort => SupportedType.UShort,
                int => SupportedType.Int,
                uint => SupportedType.UInt,
                long => SupportedType.Long,
                ulong => SupportedType.ULong,
                float => SupportedType.Float,
                double => SupportedType.Double,
                decimal => SupportedType.Decimal,
                string => SupportedType.String,
                Vector2 => SupportedType.Vector2,
                Vector3 => SupportedType.Vector3,
                _ => SupportedType.Bool

            };
        }

        public static SupportedType[] ReadTypes(this BinaryReader reader, int objectCount)
        {
            SupportedType[] types = new SupportedType[objectCount];
            int index = 0;

            while (index < objectCount)
            {
                // Legge un byte dal flusso
                byte packedByte = reader.ReadByte();

                //Get the first 4 bit for the first type
                SupportedType type1 = (SupportedType)(packedByte & 0x0F);
                types[index++] = type1;

                if (index < objectCount)
                {
                    // Get the last 4 bit for the last type
                    SupportedType type2 = (SupportedType)((packedByte >> 4) & 0x0F);
                    types[index++] = type2;
                }
            }

            return types;
        }

        public static object[] ReadObjects(this BinaryReader binaryReader)
        {
            int length = binaryReader.ReadInt32();

            SupportedType[] types = binaryReader.ReadTypes(length);
            object[] array = new object[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = ReadObject(binaryReader, types[i]);
            }

            return array;
        }

        public static object ReadObject(this BinaryReader binaryReader, SupportedType type)
        {
            // Leggi il valore corrispondente in base al tipo
            switch (type)
            {
                case SupportedType.Bool:
                    return binaryReader.ReadBoolean();
                case SupportedType.Byte:
                    return binaryReader.ReadByte();
                case SupportedType.SByte:
                    return binaryReader.ReadSByte();
                case SupportedType.Char:
                    return binaryReader.ReadChar();
                case SupportedType.Short:
                    return binaryReader.ReadInt16();
                case SupportedType.UShort:
                    return binaryReader.ReadUInt16();
                case SupportedType.Int:
                    return binaryReader.ReadInt32();
                case SupportedType.UInt:
                    return binaryReader.ReadUInt32();
                    break;
                case SupportedType.Long:
                    return binaryReader.ReadInt64();
                case SupportedType.ULong:
                    return binaryReader.ReadUInt64();
                case SupportedType.Float:
                    return binaryReader.ReadSingle();
                case SupportedType.Double:
                    return binaryReader.ReadDouble();
                case SupportedType.Decimal:
                    return binaryReader.ReadDecimal();
                case SupportedType.String:
                    return binaryReader.ReadString();
                case SupportedType.Vector2:
                    return binaryReader.ReadVector2();
                case SupportedType.Vector3:
                    return binaryReader.ReadVector3();
                default:
                    UnityEngine.Debug.LogError("Found unsupported type " + type);
                    return null;
            }
        }


        public static void Write(this BinaryWriter writer, object value)
        {
            if (value == null)
            {
                UnityEngine.Debug.LogError("Cannot serialize null value");
                return;
            }

            if (value is bool typedValueBool)
            {
                writer.Write(typedValueBool);
            }
            else if (value is byte typedValueByte)
            {
                writer.Write(typedValueByte);
            }
            else if (value is sbyte typedValueSByte)
            {
                writer.Write(typedValueSByte);
            }
            else if (value is char typedValueChar)
            {
                writer.Write(typedValueChar);
            }
            else if (value is short typedValueShort)
            {
                writer.Write(typedValueShort);
            }
            else if (value is ushort typedValueUShort)
            {
                writer.Write(typedValueUShort);
            }
            else if (value is int typedValueInt)
            {
                writer.Write(typedValueInt);
            }
            else if (value is uint typedValueUInt)
            {
                writer.Write(typedValueUInt);
            }
            else if (value is long typedValueLong)
            {
                writer.Write(typedValueLong);
            }
            else if (value is ulong typedValueULong)
            {
                writer.Write(typedValueULong);
            }
            else if (value is float typedValueFloat)
            {
                writer.Write(typedValueFloat);
            }
            else if (value is double typedValueDouble)
            {
                writer.Write(typedValueDouble);
            }
            else if (value is decimal typedValueDecimal)
            {
                writer.Write(typedValueDecimal);
            }
            else if (value is string typedValueString)
            {
                writer.Write(typedValueString);
            }
            else if (value is Vector2 typedValueVector2)
            {
                writer.Write(typedValueVector2);
            }
            else if (value is Vector3 typedValueVector3)
            {
                writer.Write(typedValueVector3);
            }
            else
            {
                Debug.LogError("Unable to write values of type: " + value.GetType());
            }
        }

        public static void Write(this BinaryWriter binaryWriter, Dictionary<byte, object> dictionary)
        {
            //we can only read dictionary with a max number of elements of 255
            binaryWriter.Write((byte)dictionary.Count);
            var keys = dictionary.Keys.ToArray();
            for (int i = 0; i < keys.Length; i += 2)
            {
                SupportedType type1 = GetSupportedType(dictionary[keys[i]]);
                if ((i + 1 < keys.Length))
                {
                    // Type 2 is present only if we do not exceed the array count
                    binaryWriter.Write(keys[i]);
                    binaryWriter.Write(keys[i + 1]);
                    SupportedType type2 = GetSupportedType(dictionary[keys[i + 1]]);
                    binaryWriter.WriteTypes(type1, type2);
                    binaryWriter.Write(dictionary[keys[i]]);
                    binaryWriter.Write(dictionary[keys[i + 1]]);
                }
                else
                {
                    binaryWriter.Write(keys[i]);
                    binaryWriter.WriteTypes(type1, SupportedType.Bool);
                    binaryWriter.Write(dictionary[keys[i]]);
                }
            }
        }

        public static void Write(this BinaryWriter binaryWriter, Dictionary<int, object> dictionary)
        {
            binaryWriter.Write(dictionary.Count);
            var keys = dictionary.Keys.ToArray();
            for (int i = 0; i < keys.Length; i += 2)
            {
                SupportedType type1 = GetSupportedType(dictionary[keys[i]]);
                if ((i + 1 < keys.Length))
                {
                    // Type 2 is present only if we do not exceed the array count
                    binaryWriter.Write(keys[i]);
                    binaryWriter.Write(keys[i + 1]);
                    SupportedType type2 = GetSupportedType(dictionary[keys[i + 1]]);
                    binaryWriter.WriteTypes(type1, type2);
                    binaryWriter.Write(dictionary[keys[i]]);
                    binaryWriter.Write(dictionary[keys[i + 1]]);
                    UnityEngine.Debug.LogError("Serialized values " + dictionary[keys[i]] + " | " + dictionary[keys[i + 1]]);
                }
                else
                {
                    binaryWriter.Write(keys[i]);
                    binaryWriter.WriteTypes(type1, SupportedType.Bool);
                    binaryWriter.Write(dictionary[keys[i]]);
                    UnityEngine.Debug.LogError("Serialized value " + dictionary[keys[i]]);
                }
            }
        }

        public static Dictionary<int, object> ReadDictionaryInt32Object(this BinaryReader binaryReader)
        {
            int count = binaryReader.ReadInt32();

            Dictionary<int, object> dictionary = new Dictionary<int, object>();

            for (int i = 0; i < count; i += 2)
            {
                if ((i + 1 < count))
                {
                    int key1 = binaryReader.ReadInt32();
                    int key2 = binaryReader.ReadInt32();
                    SupportedType[] types = binaryReader.ReadTypes(2);
                    dictionary[key1] = binaryReader.ReadObject(types[0]);
                    dictionary[key2] = binaryReader.ReadObject(types[1]);
                }
                else
                {
                    int key = binaryReader.ReadInt32();
                    SupportedType[] types = binaryReader.ReadTypes(1);
                    dictionary[key] = binaryReader.ReadObject(types[0]);
                }
            }
            return dictionary;

        }

        public static Dictionary<byte, object> ReadDictionaryByteObject(this BinaryReader binaryReader)
        {
            //we can only read dictionary with a max number of elements of 255
            byte count = binaryReader.ReadByte();

            Dictionary<byte, object> dictionary = new Dictionary<byte, object>();

            for (int i = 0; i < count; i += 2)
            {
                if ((i + 1 < count))
                {
                    byte key1 = binaryReader.ReadByte();
                    byte key2 = binaryReader.ReadByte();
                    SupportedType[] types = binaryReader.ReadTypes(2);
                    dictionary[key1] = binaryReader.ReadObject(types[0]);
                    dictionary[key2] = binaryReader.ReadObject(types[1]);
                }
                else
                {
                    byte key = binaryReader.ReadByte();
                    SupportedType[] types = binaryReader.ReadTypes(1);
                    dictionary[key] = binaryReader.ReadObject(types[0]);
                }
            }
            return dictionary;

        }
    }
}
