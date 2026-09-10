using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        /// <summary>
        /// Remove all special characters from a string leaving only letters and numbers and the characters inside the parameter includeCharacters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(this string str, string includeCharacters = "")
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || includeCharacters.Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpaces(this string str)
        {
            str = Regex.Replace(str, @"\s", string.Empty);
            return str;
        }


        public static string ConcatenateStrings(this IEnumerable<string> list, char separator)
        {
            // Use StringBuilder for efficient string concatenation
            StringBuilder sb = new StringBuilder();

            // Iterate through each element in the list
            foreach (string element in list)
            {
                // Append the current element to the StringBuilder
                sb.Append(element);

                // Append the separator character after each element

                sb.Append(separator);

            }
            //remove separator from the last element
            if (sb.Length > 0)
            {
                // remove last string character
                sb.Remove(sb.Length - 1, 1);
            }
            // Return the resulting string
            return sb.ToString();
        }

        public static string CompressString(this string text)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(text);

            using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gzipStream.Write(byteArray, 0, byteArray.Length);
                }

                // Convertiamo i dati compressi in una stringa Base64
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        // Metodo per decomprimere una stringa compressa
        public static string DecompressString(this string compressedText)
        {
            byte[] compressedByteArray = Convert.FromBase64String(compressedText);

            using (var memoryStream = new MemoryStream(compressedByteArray))
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var decompressedMemoryStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(decompressedMemoryStream);
                        byte[] decompressedByteArray = decompressedMemoryStream.ToArray();

                        // Convertiamo i dati decompressi in una stringa
                        return Encoding.UTF8.GetString(decompressedByteArray);
                    }
                }
            }
        }

        // Metodo per cifrare una stringa usando AES e PBKDF2 per derivare la chiave
        private static byte[] GetSha256Key(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password)); // Ottiene una chiave a 256 bit (32 byte)
            }
        }

        // Metodo per cifrare una stringa usando AES con SHA-256
        public static string EncryptAES_SHA256(this string plainText, string password, byte[] IV = null)
        {
            byte[] key = GetSha256Key(password);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                if (IV == null)
                {
                    IV = new byte[16]; // IV di 16 byte inizializzato a zero
                }
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Metodo per decifrare una stringa usando AES con SHA-256
        public static string DecryptAES_SHA256(this string cipherText, string password, byte[] IV = null)
        {
            byte[] key = GetSha256Key(password);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                if (IV == null)
                {
                    IV = new byte[16];
                }
                aesAlg.IV = IV; // L'IV deve essere lo stesso usato per cifrare

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        /// <summary>
        /// Alphanumeric characters as a power of 2 (64 chars)
        /// These must be a power of 2 to ensure that xor operation won't exceed the count value
        /// </summary>
        private static readonly string AlphanumericChars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz?!";

        /// <summary>
        /// Encrypt and Decrypt an alphanumeric string with the xor cypher and the given key
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>

        public static string EncryptDecriptXOR(this string input, string key)
        {
            var output = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                //Find the position of the character so that we get a byte of maximum value AlphanumericChars.Length
                byte inputCharacterToInt = (byte)AlphanumericChars.IndexOf(input[i]);
                byte keyCharacterToInt = (byte)AlphanumericChars.IndexOf(key[i % key.Length]);
                //The xor will give us a value lower than AlphanumericChars.Length if the lenght is a power of 2
                byte xor = (byte)(inputCharacterToInt ^ keyCharacterToInt);

                output.Append(AlphanumericChars[xor]);
            }
            return output.ToString();
        }

        public static string GenerateRandomAlphanumericString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new(length);
            System.Random random = new();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}