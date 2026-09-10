using System.Collections.Generic;

namespace Virtuademy.SDK.Core.Editor
{
    public static class AssetDatabaseExtension
    {
        public static List<T> SearchAssetByType<T>() where T : UnityEngine.Object
        {
            string typeName = typeof(T).Name;
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeName}");
            List<T> assets = new List<T>();
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }
            return assets;
        }
    }
}
