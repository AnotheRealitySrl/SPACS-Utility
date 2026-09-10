using System;
using System.Collections.Generic;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
#endif

namespace Virtuademy.SDK.Core.Utilities
{
    public class AndroidDeepLinkParser : UrlParametersParserBase
    {
        public override Dictionary<string, string> ParseUrlParameters()
        {
            Dictionary<string, string> queryParams = new();

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");

            if (uri != null)
            {
                string deepLinkUrl = uri.Call<string>("toString");
                Debug.Log("App opened with deep linkg: " + deepLinkUrl);

                queryParams = ParseQueryString(deepLinkUrl);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Errore nel recuperare il deep link: " + ex.Message);
        }
#endif

            return queryParams;
        }

        /// <summary>
        /// Esegue il parsing della querystring da un URL usando le librerie standard .NET.
        /// È più robusto e leggibile rispetto all'iterazione tramite AndroidJavaObject.
        /// </summary>
        /// <param name="url">L'URL completo da cui estrarre i parametri.</param>
        /// <returns>Un dizionario con i parametri della querystring.</returns>
        private Dictionary<string, string> ParseQueryString(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new Dictionary<string, string>();

            int qIndex = url.IndexOf('?');
            if (qIndex < 0 || qIndex == url.Length - 1)
                return new Dictionary<string, string>();

            // Cut off fragment (#...) if present
            int fragmentIndex = url.IndexOf('#', qIndex + 1);
            string query = fragmentIndex >= 0
                ? url.Substring(qIndex + 1, fragmentIndex - qIndex - 1)
                : url.Substring(qIndex + 1);

            var result = new Dictionary<string, string>();

            // Fast path: iterate without allocating intermediate arrays unnecessarily
            var pairs = query.Split('&');
            foreach (var pair in pairs)
            {
                if (string.IsNullOrEmpty(pair))
                    continue;

                int eq = pair.IndexOf('=');
                string rawKey, rawValue;

                if (eq >= 0)
                {
                    rawKey = pair.Substring(0, eq);
                    rawValue = pair.Substring(eq + 1);
                }
                else
                {
                    rawKey = pair;
                    rawValue = string.Empty;
                }

                string key = DecodeQueryComponent(rawKey);
                string value = DecodeQueryComponent(rawValue);

                // If duplicate keys can appear you may:
                // 1) Overwrite (current behavior)
                // 2) Keep first: if (!result.ContainsKey(key)) result[key] = value;
                // 3) Store lists (change signature)
                result[key] = value;
            }

            return result;
        }

        private string DecodeQueryComponent(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            // + means space in application/x-www-form-urlencoded
            s = s.Replace("+", " ");

            try
            {
                return Uri.UnescapeDataString(s);
            }
            catch
            {
                // Fallback if malformed percent-encoding
                return s;
            }
        }
    }
}
