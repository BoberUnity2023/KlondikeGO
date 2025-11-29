#if VK
using System.Collections;
using System.Collections.Generic;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace BloomLines.Saving.Adapters
{
    public class VKAdapter : BaseAdapter
    {
        public override void Initialize()
        {
            Debug.Log("VKAdapter.Initing...");
            UpdateHelper.Get().StartCoroutine(LoadStorage());            
            UpdateHelper.Get().StartCoroutine(BannerLogic(30));
            Debug.Log("VKAdapter.Inited");
        }

        private IEnumerator LoadStorage()
        {
            yield return new WaitForSeconds(1f);
            Application.ExternalCall("storageGet", VKController.StorageKey);
        }

        public override void Sync()
        {
            Debug.Log("VK Sync");

            var json = JsonUtility.ToJson(VKController.Storage);
            string compressedJson = StringCompressor.CompressStringBrotli(json);

            Application.ExternalCall("storageSet", VKController.StorageKey, compressedJson);
        }

        private IEnumerator BannerLogic(float time)
        {
            Debug.Log("VKAdapter.BannerLogic start");
            yield return new WaitForSeconds(3f);            
            var delay = new WaitForSeconds(time);            
            var gameState = SaveManager.GameState;            
            if (!gameState.Purchased.Contains(IAPController.NO_ADS))
            {                
                ShowBanners();

                while (true)
                {
                    yield return delay;

                    if (gameState.Purchased.Contains(IAPController.NO_ADS))
                        break;

                    ShowBanners();
                    UpdateHelper.Get().StartCoroutine(BannerLogic(65));
                }
            }            
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

        public override bool HasSave<T>()
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                return !string.IsNullOrEmpty(VKController.Storage.GameStateJson);
            }
            else if (type == typeof(GameModeState))
            {
                return !string.IsNullOrEmpty(VKController.Storage.GameModeStateJson);
            }

            return false;
        }

        protected override string LoadJson<T>()
        {
            var type = typeof(T);
            string json = string.Empty;

            if (type == typeof(GameState))
            {
                json = VKController.Storage.GameStateJson;
            }
            else if (type == typeof(GameModeState))
            {
                json = VKController.Storage.GameModeStateJson;
            }

            return json;
        }

        protected override void SaveJson<T>(string json)
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                VKController.Storage.GameStateJson = json;
            }
            else if (type == typeof(GameModeState))
            {
                VKController.Storage.GameModeStateJson = json;
            }
        }
    }
}
#endif