#if GAME_PUSH
using GamePush;
using UnityEngine;

namespace BloomLines.Saving.Adapters
{
    public class GamePushAdapter : BaseAdapter
    {
        public override void Initialize()
        {
        }

        // Синхронизируем данные в GamePush облако
        public override void Sync()
        {
            Debug.Log("GamePush Sync");

            GP_Player.Sync();
        }

        // Проверяем есть ли нужное сохранение в облаке
        public override bool HasSave<T>()
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                var json = GP_Player.GetString("game_state");
                return !string.IsNullOrEmpty(json);
            }
            else if (type == typeof(GameModeState))
            {
                var json = GP_Player.GetString("game_mode_state");
                return !string.IsNullOrEmpty(json);
            }

            return false;
        }

        // Загружаем нужное сохранение из облака
        protected override string LoadJson<T>()
        {
            var type = typeof(T);
            string json = string.Empty;

            if (type == typeof(GameState))
            {
                json = GP_Player.GetString("game_state");
            }
            else if (type == typeof(GameModeState))
            {
                json = GP_Player.GetString("game_mode_state");
            }

            return json;
        }

        // Сохраняем нужное в облако
        protected override void SaveJson<T>(string json)
        {
            var type = typeof(T);

            if (type == typeof(GameState))
            {
                GP_Player.Set("game_state", json);
            }
            else if (type == typeof(GameModeState))
            {
                GP_Player.Set("game_mode_state", json);
            }
        }
    }
}
#endif