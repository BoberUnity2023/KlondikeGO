#if OK
using System.Collections;
using BloomLines.Controllers;
using BloomLines.Helpers;
using UnityEngine;

namespace BloomLines.Saving.Adapters
{
    public class OKAdapter : BaseAdapter
    {
        public override void Initialize()
        {
            UpdateHelper.Get().StartCoroutine(LoadStorage());
        }

        private IEnumerator LoadStorage()
        {
            yield return new WaitForSeconds(1f);
            Application.ExternalCall("loadFromOKStorage", OKController.StorageKey);
        }

        public override void Sync()
        {
            Debug.Log("OK Sync");

            var json = JsonUtility.ToJson(OKController.Storage);
            string compressedJson = StringCompressor.CompressStringBrotli(json);

            Application.ExternalCall("saveToOKStorage", OKController.StorageKey, compressedJson);
        }

        public override bool HasSave<T>()
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                return !string.IsNullOrEmpty(OKController.Storage.GameStateJson);
            }
            else if (type == typeof(GameModeState))
            {
                return !string.IsNullOrEmpty(OKController.Storage.GameModeStateJson);
            }

            return false;
        }

        protected override string LoadJson<T>()
        {
            var type = typeof(T);
            string json = string.Empty;

            if (type == typeof(GameState))
            {
                json = OKController.Storage.GameStateJson;
            }
            else if (type == typeof(GameModeState))
            {
                json = OKController.Storage.GameModeStateJson;
            }

            return json;
        }

        protected override void SaveJson<T>(string json)
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                OKController.Storage.GameStateJson = json;
            }
            else if (type == typeof(GameModeState))
            {
                OKController.Storage.GameModeStateJson = json;
            }
        }
    }
}
#endif