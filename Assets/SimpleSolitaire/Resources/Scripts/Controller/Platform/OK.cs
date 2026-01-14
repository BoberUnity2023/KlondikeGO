using BloomLines.Helpers;
using System.Collections;
using UnityEngine;


namespace BloomLines
{
    public class OK : PlatformController
    {        
#if OK
        private void Start()
        {
            InitOK();
            StartCoroutine(LoadStorage());            
        }

        private void InitOK()
        {
            Debug.Log("OKController.Initialize()");
            Application.ExternalCall("OKInit");
        }

        private IEnumerator LoadStorage()
        {
            yield return new WaitForSeconds(0.2f);
            Application.ExternalCall("loadFromOKStorage", "json");
#if UNITY_EDITOR
            IsSaveLoaded = true;
            Json = "";
#endif
        }

        public void OnGetStorage(string json)//from index.html
        {
            Debug.Log("OK Storage Loaded success: " + json);

            if (json == "error")
            {
                Debug.Log("OK Json error. LoadDefault");
                Json = "";
            }

            if (!string.IsNullOrEmpty(json))
            {
                string decompressedJson = StringCompressor.DecompressStringBrotli(json);
                Json = decompressedJson;
            }
            else
            {
                Debug.Log("Empty OK Save");
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

        public void OnGetTargetPlatform(string arg1)//from index.html
        {
            if (string.IsNullOrEmpty(arg1)) return;
            
            Debug.Log("OK TargetPlatform: " + arg1);
            if (arg1 == "ios" | arg1 == "iosweb")
            {
                //IOSInitialized?.Invoke();
            }
        }

        public void OnGetCallback(string arg1)//from index.html
        {
            if (string.IsNullOrEmpty(arg1))
                return;

            Debug.Log("OK Callback: " + arg1);
        }

        protected override bool IsSingle
        {
            get 
            {
                OK[] oks = FindObjectsByType<OK>(FindObjectsSortMode.None);

                if (oks.Length > 1)
                    return false;
                else
                    return true;
            }
        }
#endif
    }
}