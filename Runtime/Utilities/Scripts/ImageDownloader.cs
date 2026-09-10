using Virtuademy.SDK.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Virtuademy.SDK.Core.Utilities
{
    public static class ImageDownloader
    {
        private static Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

        public static async void DownloadImage(string mediaUrl, Action<Texture2D> onCompletionCallback, Action onFailedCallback = null, string key = null)
        {
            try
            {
                Texture2D texture = await DownloadImageAsync(mediaUrl, key);
                onCompletionCallback(texture);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onFailedCallback();
            }
        }

        public static async Task<Texture2D> DownloadImageAsync(string mediaUrl, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = mediaUrl;
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Trying to download an image with an empty url!");
            }
            if (imageCache.TryGetValue(key, out Texture2D texture))
            {
                //The image key is present in the dictionary return the value
                if (texture != null)
                {
                    return texture;
                }
                // the image key is present but the download is not not completed yet
                // wait for download completion
                else
                {
                    while (!(!imageCache.ContainsKey(key) || (imageCache.TryGetValue(key, out texture) && texture != null)))
                    {
                        await Task.Yield();
                    }
                    if (!imageCache.ContainsKey(key))
                    {
                        throw new Exception("Failed to download image: " + key + ".");
                    }
                    return texture;
                }
            }
            else
            {
                //Add image key to cache and start download
                imageCache.Add(key, null);
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
                await request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    imageCache.Remove(mediaUrl);
                    throw new Exception("Failed to download image: " + mediaUrl + ". " + request.error);
                }
                else
                {
                    texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    imageCache[key] = texture;
                    return texture;
                }
            }
        }

    }
}
