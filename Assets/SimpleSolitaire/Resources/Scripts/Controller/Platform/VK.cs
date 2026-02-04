using BloomLines.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloomLines
{
    public class VK : PlatformController
    {
#if VK
        private void Start()
        {
            InitBridge();
            //StartCoroutine(LoadStorage());//StorageVK
            StartCoroutine(BannerLogic(30));
        }        

        private void InitBridge()
        {
            Application.ExternalCall("initBridge");
        }

        private IEnumerator LoadStorage()
        {
            yield return new WaitForSeconds(0.1f);
            Application.ExternalCall("storageGet", "json");

#if UNITY_EDITOR
            IsSaveLoaded = true;
            Json = "";
#endif
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

        public void OnGetStorage(string json)//from index.html
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
                Debug.Log("VK Storage Decompressed success: " + decompressedJson);
                Json = decompressedJson;
            }
            else
            {
                Debug.Log("Empty VK Save");
                Json = "";
            }
            IsSaveLoaded = true;
        }

        public void SendResult(string text)
        {
            ResultBanners(text);
        }

        public void ResultBanners(string text)
        {
            Debug.Log("ResultBanners: " + text);
        }

        protected override bool IsSingle
        {
            get
            {
                VK[] vks = FindObjectsByType<VK>(FindObjectsSortMode.None);

                if (vks.Length > 1)
                    return false;
                else
                    return true;
            }
        }
#endif
    }
}