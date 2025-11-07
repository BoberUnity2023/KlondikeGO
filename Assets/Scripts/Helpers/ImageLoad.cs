using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BloomLines.Helpers
{
    public static class ImageLoad
    {
        public static void Load(string url, Action<Texture2D> onLoaded)
        {
            var update = UpdateHelper.Get();
            update.StartCoroutine(LoadTexture(url, onLoaded));
        }

        private static IEnumerator LoadTexture(string url, Action<Texture2D> onLoaded)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogError("ImageLoad Error: " + webRequest.error);
                }
                else
                {
                    DownloadHandlerTexture handlerTexture = webRequest.downloadHandler as DownloadHandlerTexture;

                    if (handlerTexture.isDone)
                    {
                        onLoaded?.Invoke(handlerTexture.texture);
                    }
                }
            }
        }
    }
}