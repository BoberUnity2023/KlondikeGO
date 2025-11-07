#if OK
using System;
using System.Collections;
using BloomLines.Helpers;
using BloomLines.Saving;
using UnityEngine;

namespace BloomLines.Controllers
{
    [Serializable]
    public struct GetPageInfoStruct
    {
        public string method;
        public string result;

        public static GetPageInfoStruct CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<GetPageInfoStruct>(jsonString);
        }
    }

    [System.Serializable]
    public class OKStorage
    {
        public string GameStateJson;
        public string GameModeStateJson;
    }

    public static class OKController
    {
        public static OKStorage Storage { get; private set; }
        public static bool SaveLoaded { get; private set; }

        public const string StorageKey = "save";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var obj = new GameObject("OKHandle");
            var handle = obj.AddComponent<OKHandle>();

            handle.onGetStorage += OnGetStorage;
            handle.onGetCallback += OnGetCallback;
            handle.onGetTargetPlatform += OnGetTargetPlatform;

            GameObject.DontDestroyOnLoad(obj);

            Application.ExternalCall("OKInit");
        }

        private static void OnGetStorage(string arg1)
        {
            Storage = new OKStorage();

            if (!string.IsNullOrEmpty(arg1))
            {
                Debug.Log("Œ  Save Loaded");

                string decompressedJson = StringCompressor.DecompressStringBrotli(arg1);
                Storage = JsonUtility.FromJson<OKStorage>(decompressedJson);
            }
            else
            {
                Debug.Log("Empty OK Save");
            }

            SaveLoaded = true;
        }

        private static void OnGetTargetPlatform(string arg1)
        {
            if (string.IsNullOrEmpty(arg1)) return;

            if (arg1 == "ios" | arg1 == "iosweb")
            {
                //IOSInitialized?.Invoke();
            }
        }

        private static void OnGetCallback(string arg1)
        {
            if (string.IsNullOrEmpty(arg1))
                return;

            Debug.Log("OK Callback: " + arg1);
        }
    }
}
#endif