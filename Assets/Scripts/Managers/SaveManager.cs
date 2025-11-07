using BloomLines.Assets;
using BloomLines.Saving;
using BloomLines.Saving.Adapters;
using BloomLines.Skins;
using BloomLines.UI;
using I2.Loc;
using UnityEngine;
using BloomLines.Controllers;
using System.Collections.Generic;


#if CONSOLE
using QFSW.QC;
#endif

namespace BloomLines.Managers
{
    // Тип сохранения, игры или уровня
    public enum SaveType
    {
        Game,
        GameMode,
    }

    // Фаза сохранения игры. Подготовка или завершение
    public enum SavePhase
    {
        Prepare,
        Completed,
    }

    // Ивент при сохранении
    public class SaveEvent
    {
        public SaveType Type { get; private set; }
        public SavePhase Phase { get; private set; }

        public SaveEvent(SaveType type, SavePhase phase)
        {
            Type = type; 
            Phase = phase;
        }
    }

    // Ивент для проверки можно ли сохранить текущий тип сохранения
    public class CheckCanSaveEvent
    {
        public SaveType SaveType { get; private set; }

        public bool CanSave;

        public CheckCanSaveEvent(SaveType saveType)
        {
            SaveType = saveType;
            CanSave = true;
        }
    }

    public static class SaveManager
    {
        public static GameState GameState { get; private set; }
        public static GameModeState GameModeState { get; private set; }
        public static bool SaveLoaded { get; private set; }

        private static ISaveAdapter _localSaveAdapter; // Адаптер локального сохранения
        private static ISaveAdapter _cloudSaveAdapter; // Адаптер облачного сохранения

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SaveLoaded = false;

            _localSaveAdapter = new LocalAdapter();

#if UNITY_EDITOR
            return;
#endif

#if Yandex
            _cloudSaveAdapter = new YandexAdapter();
#endif

#if VK
            _cloudSaveAdapter = new VKAdapter();
#endif

#if OK
            _cloudSaveAdapter = new OKAdapter();
#endif

            _localSaveAdapter.Initialize();
            if(_cloudSaveAdapter != null)
                _cloudSaveAdapter.Initialize();
        }

        // Выставляем сохранение уровня
        public static void SetGameModeState(GameModeState state)
        {
            GameModeState = state;
        }

        // Загружаем все сохранения
        public static void LoadAll()
        {
            GameState = LoadLastSaveState<GameState>();

            if(HasSaveState<GameModeState>())
                GameModeState = LoadLastSaveState<GameModeState>();

            SaveLoaded = true;
        }

        // Сохраняем нужный тип данных
        public static void Save<T>(SaveType type, T state) where T : SaveState
        {
            if (state == null)
                return;

            var checkCanSaveEvent = new CheckCanSaveEvent(type);
            EventsManager.Publish(checkCanSaveEvent);

            if (!checkCanSaveEvent.CanSave) // Проверяем можно ли сохранить этот тип сохранения
                return;

            var saveEventPrepare = new SaveEvent(type, SavePhase.Prepare); // Отсылаем ивент что игра готовиться к сохранению
            EventsManager.Publish(saveEventPrepare);

            _localSaveAdapter.Save(state); // Локально сохраняем

            if (_cloudSaveAdapter != null) // Облачно сохраняем
                _cloudSaveAdapter.Save(state);

            var saveEventCompleted = new SaveEvent(type, SavePhase.Completed); // Отсылаем ивент что игра сохранилась
            EventsManager.Publish(saveEventCompleted);
        }

        // Проверка есть ли нужный тип сохранения
        private static bool HasSaveState<T>() where T : SaveState
        {
            if (_cloudSaveAdapter != null && _cloudSaveAdapter.HasSave<T>())
                return true;

            return _localSaveAdapter.HasSave<T>();
        }

        // Загрузить последнее актуально сохранение нужного типа
        private static T LoadLastSaveState<T>() where T : SaveState
        {
            T cloudState = null;
            T localState = null;

            if (_cloudSaveAdapter != null && _cloudSaveAdapter.HasSave<T>())
                cloudState = _cloudSaveAdapter.Load<T>();

            localState = _localSaveAdapter.Load<T>();

            if (cloudState == null || localState.SaveTime > cloudState.SaveTime) // В приоритете загрузки, сохранение которое более актуальное
            {
                Debug.Log($"Loaded LocalSave '{typeof(T).Name}'");
                return localState;
            }
            else
            {
                Debug.Log($"Loaded CloudSave '{typeof(T).Name}'");
                return cloudState;
            }
        }

        // Получить стандартный шаблон сохранения нужного типа
        public static T GetDefaultState<T>() where T : SaveState
        {
            if (typeof(T) == typeof(GameState)) // Шаблон для сохранения игры
            {
                var gameState = new GameState();
                gameState.SaveVersion = Application.version;
                gameState.Vibration = true;
                gameState.MusicVolume = 0.8f;
                gameState.SoundVolume = 0.8f;
                gameState.SkinPack = "skin_pack_1";
                gameState.LanguageCode = (LanguageCode)System.Enum.Parse(typeof(LanguageCode), LocalizationManager.CurrentLanguageCode.ToUpperInvariant());

                gameState.CompletedTutorials.Add("first_show_secateurs");

                return (T)(object)gameState;
            }
            else if (typeof(T) == typeof(GameModeState)) // Шаблон для сохранения уровня
            {
                var gameModeState = new GameModeState();
                gameModeState.SaveVersion = Application.version;
                gameModeState.BoardState = new BoardState();

                return (T)(object)gameModeState;
            }

            return null;
        }

        // Синхронизировать облачное сохранение
        public static void Sync()
        {
            _localSaveAdapter.Sync();

            if (_cloudSaveAdapter != null)
                _cloudSaveAdapter.Sync();
        }

#if CONSOLE
        [Command("save")]
#endif
        public static void Save(SaveType type)
        {
            switch (type)
            {
                case SaveType.Game:
                    Save(type, GameState);
                    break;
                case SaveType.GameMode:
                    Save(type, GameModeState);
                    break;
            }
        }

#if CONSOLE
        [Command("set_skin_pack")]
        private static void SetSkinPack(string id)
        {
            var skinPack = GameAssets.GetSkinPackData(id);
            if(skinPack == null)
            {
                Debug.LogError("Missing SkinPack with id: " + id);
                return;
            }

            GameState.SkinPack = id;
            EventsManager.Publish(new UpdateSkinPackEvent());
        }

        [Command("set_task_stage")]
        private static void SetTaskStage(int stage)
        {
            if(GameModeState != null)
            {
                var balanceData = GameAssets.BalanceData;
                GameModeState.CompletedTasksCount = stage * balanceData.TasksCountInStage;
            }
        }

        [Command("add_coins")]
        private static void AddCoins(int count)
        {
            if (GameModeState != null)
            {
                EventsManager.Publish(new AddedScoreEvent(count));
            }
        }
#endif
    }
}