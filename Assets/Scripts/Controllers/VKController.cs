#if VK
using System.Runtime.InteropServices;
using BloomLines.Helpers;
using UnityEngine;

namespace BloomLines.Controllers
{
    [System.Serializable]
    public class VKStorage
    {
        public string GameStateJson;
        public string GameModeStateJson;
    }

    public static class VKController
    {
        //[DllImport("__Internal")] public static extern void _VKWebAppStorageSet(string key, string value);
        //[DllImport("__Internal")] public static extern void _VKWebAppStorageGet(string key);

        public static VKStorage Storage { get; private set; }
        public static bool SaveLoaded { get; private set; }

        public const string StorageKey = "save";


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var obj = new GameObject("VKHandle");
            var handle = obj.AddComponent<VKHandle>();

            handle.onGetStorage += OnGetStorage;

            GameObject.DontDestroyOnLoad(obj);

            Application.ExternalCall("initBridge");
        }

        private static void OnGetStorage(string arg1)
        {
            Storage = new VKStorage();

            if (!string.IsNullOrEmpty(arg1))
            {
                Debug.Log("VK Save Loaded");

                string decompressedJson = StringCompressor.DecompressStringBrotli(arg1);
                Storage = JsonUtility.FromJson<VKStorage>(decompressedJson);
            }
            else
            {
                Debug.Log("Empty VK Save");
            }

            SaveLoaded = true;
        }

//        public void VKWebAppStorageSet(string key, string value)
//        {
//#if !UNITY_EDITOR
//                _VKWebAppStorageSet(key, value);
//#endif
//        }

//        public void VKWebAppStorageGet(string key, UnityAction<string> action)
//        {
//#if !UNITY_EDITOR
//                 TODO : Вместо стринга сделать отдельную структуру, в которой будет отслеживаться ошибка
//                _actionStorageGet = action;
//                _VKWebAppStorageGet(key);
//#endif
//        }
    }
}
#endif