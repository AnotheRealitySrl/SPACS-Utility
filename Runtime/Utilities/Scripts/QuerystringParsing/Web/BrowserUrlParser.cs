using System;
using System.Collections.Generic;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices; // Necessario per DllImport
#endif

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public class BrowserUrlParser : UrlParametersParserBase
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            [DllImport("__Internal")]
            private static extern string GetQueryString();
#endif

        public override Dictionary<string, string> ParseUrlParameters()
        {
            var queryParams = new Dictionary<string, string>();
            string queryString = "";

#if UNITY_WEBGL && !UNITY_EDITOR
                queryString = GetQueryString();
#endif

            if (!string.IsNullOrEmpty(queryString) && queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                string[] parameters = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string param in parameters)
                {
                    int eqIndex = param.IndexOf('=');
                    if (eqIndex > 0)
                    {
                        string rawKey = param.Substring(0, eqIndex);
                        string rawValue = param.Substring(eqIndex + 1); // keep remaining (could include '=' as in base64)
                        string key = SafeDecode(rawKey);
                        string value = SafeDecode(rawValue);
                        queryParams[key] = value;
                    }
                    else
                    {
                        // Optionally treat key without '=' as flag:
                        // queryParams[SafeDecode(param)] = "";
                    }
                }
            }

            foreach (var kvp in queryParams)
            {
                Debug.Log($"{nameof(BrowserUrlParser)}: Key: {kvp.Key}, Value: {kvp.Value}");
            }

            return queryParams;
        }

        private static string SafeDecode(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            try
            {
                // Uri.UnescapeDataString does NOT convert '+' to space; it only processes percent-encodings.
                return Uri.UnescapeDataString(s);
            }
            catch
            {
                return s; // Fallback: return original if malformed.
            }
        }
    }
}

