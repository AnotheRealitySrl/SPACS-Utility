#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

using UnityEngine;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class BrowserStorage
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Importa le funzioni dal file .jslib
        [DllImport("__Internal")]
        private static extern void SaveDataToSessionStorage(string key, string value);

        [DllImport("__Internal")]
        private static extern string LoadDataFromSessionStorage(string key);
#endif

        public static void Save(string key, string value)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveDataToSessionStorage(key, value);
#endif
        }

        public static string Load(string key)
        {
            string data = null;

#if UNITY_WEBGL && !UNITY_EDITOR
        // Nota: la gestione della memoria è complessa.
        // Il codice .jslib sopra alloca memoria che C# deve liberare.
        // Un approccio più semplice è quando JS ritorna una stringa
        // come nel nostro "LoadDataFromSessionStorage".
        // Unity gestisce la conversione del puntatore (buffer) in stringa.
        data = LoadDataFromSessionStorage(key);
#endif

            Debug.Log("Load function called in non-WebGL environment. Key: " + data);
            return data;
        }
    }

}
