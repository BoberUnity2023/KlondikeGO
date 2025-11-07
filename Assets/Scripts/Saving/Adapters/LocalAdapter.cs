using UnityEngine;

namespace BloomLines.Saving.Adapters
{
    public class LocalAdapter : BaseAdapter
    {
        public override void Initialize()
        {
        }

        public override void Sync()
        {
            PlayerPrefs.Save();
        }

        // Локально проверяем есть ли сохранение
        public override bool HasSave<T>()
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                return PlayerPrefs.HasKey("game_state");
            }
            else if (type == typeof(GameModeState))
            {
                return PlayerPrefs.HasKey("game_mode_state");
            }

            return false;
        }

        // Локально загружаем 
        protected override string LoadJson<T>()
        {
            var type = typeof(T);
            string json = string.Empty;

            if (type == typeof(GameState))
            {
                json = PlayerPrefs.GetString("game_state", string.Empty);
            }
            else if(type == typeof(GameModeState))
            {
                json = PlayerPrefs.GetString("game_mode_state", string.Empty);
            }

            return json;
        }

        // Локально сохраняем
        protected override void SaveJson<T>(string json)
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                PlayerPrefs.SetString("game_state", json);
            }
            else if(type == typeof(GameModeState))
            {
                PlayerPrefs.SetString("game_mode_state", json);
            }
        }
    }
}