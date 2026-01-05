using BloomLines.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BloomLines
{
    public class VK : MonoBehaviour
    {
        private SaveController _saveController;
        private int _loadedScene;
        public string Json { get; private set; }
        public bool IsSaveLoaded { get; private set; }

        private void Awake()
        {
#if !VK
    Destroy(gameObject);
#endif

#if VK
            Debug.Log("VK Awake()");
            if (IsSingle)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
#endif
        }
#if VK
        private void Start()
        {
            InitBridge();
            StartCoroutine(LoadStorage());
            StartCoroutine(BannerLogic(30));
        }

        void OnEnable()
        {            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            _loadedScene = scene.buildIndex;
            if (scene.name == "2_KlondikeGO")
            {
                _saveController = FindFirstObjectByType<SaveController>();

                if (_saveController == null)
                { 
                    Debug.LogError("VK. SaveController == null");
                    return;
                }

                _saveController.OnGetStorageVK(Json);
            }
        }

        private void InitBridge()
        {
            Application.ExternalCall("initBridge");            
        }

        private IEnumerator LoadStorage()
        {
            yield return new WaitForSeconds(0.2f);
            Application.ExternalCall("storageGet", "json");
        }

        private IEnumerator BannerLogic(float time)
        {
            Debug.Log("VK.BannerLogic start");
            yield return new WaitForSeconds(time);            
            if (_loadedScene == 2)
            {
                if (_saveController != null && !_saveController.Save.NoAds)
                {
                    ShowBanners();
                    StartCoroutine(BannerLogic(65));
                }
            }
            else
                StartCoroutine(BannerLogic(65));
        }

        private void ShowBanners()
        {
            Debug.Log("VK: ShowBanners");
            Dictionary<string, string> Params = new Dictionary<string, string>
            {
                {"banner_location", "top"}
            };

            ParamsStruct paramsStruct = new ParamsStruct();
            paramsStruct.Key = new string[Params.Count];
            paramsStruct.Body = new string[Params.Count];

            int Count = 0;
            foreach (KeyValuePair<string, string> Param in Params)
            {
                paramsStruct.Key[Count] = Param.Key;
                paramsStruct.Body[Count] = Param.Value;
                Count++;
            }

            Application.ExternalCall("CustomSend", "VKWebAppShowBannerAd", JsonUtility.ToJson(paramsStruct));
        }

        public void InvieFriend()
        {
            Application.ExternalCall("VKWebAppShowInviteBox");
        }

        public void OnGetStorage(string json)
        {
            Debug.Log("VK Storage Loaded success: " + json);

            if (json == "error")
            {
                Debug.Log("VK Json error. LoadDefault");
                Json = "";
            }

            if (!string.IsNullOrEmpty(json))
            {
                string decompressedJson = StringCompressor.DecompressStringBrotli(json);
                Json = decompressedJson;
            }
            else
            {
                Debug.Log("Empty VK Save");
                Json = "";
            }
            IsSaveLoaded = true;
        }


        private bool IsSingle
        {
            get
            {
                VK[] _gPCs = FindObjectsByType<VK>(FindObjectsSortMode.None);

                if (_gPCs.Length > 1)
                    return false;
                else
                    return true;
            }
        }

        public void SendResult(string text)
        {
            ResultBanners(text);
        }

        public void ResultBanners(string text)
        {
            Debug.Log("ResultBanners: " + text);
        }
#endif
    }
}