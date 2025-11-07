#if Yandex
using UnityEngine;
using YG;

namespace BloomLines.Saving.Adapters
{
    public class YandexAdapter : BaseAdapter
    {
        public override void Initialize()
        {
        }

        // Синхронизуер данные в яндекс облако
        public override void Sync()
        {
            Debug.Log("Yandex Sync");

            YG2.SaveProgress();
        }

        // Провряем есть ли нужное сохранение в яндексе
        public override bool HasSave<T>()
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                return !string.IsNullOrEmpty(YG2.saves.GameStateJson);
            }
            else if(type == typeof(GameModeState))
            {
                return !string.IsNullOrEmpty(YG2.saves.GameModeStateJson);
            }

            return false;
        }

        // Загружаем сохранение из яндекса
        protected override string LoadJson<T>()
        {
            var type = typeof(T);
            string json = string.Empty;

            if (type == typeof(GameState))
            {
                json = YG2.saves.GameStateJson;
            }
            else if (type == typeof(GameModeState))
            {
                json = YG2.saves.GameModeStateJson;
            }

            return json;
        }

        // Сохраняем в яндекс 
        protected override void SaveJson<T>(string json)
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                YG2.saves.GameStateJson = json;
            }
            else if (type == typeof(GameModeState))
            {
                YG2.saves.GameModeStateJson = json;
            }
        }
    }
}
#endif
