using BloomLines.Managers;
using BloomLines.Saving;
using UnityEngine;
using BloomLines.UI;

#if CRAZY_GAMES
using CrazyGames;
#endif

namespace BloomLines.Controllers
{
    public enum GameModeType
    {
        None,
        Classic,
        Adventure,
    }

    // Ивент для добавление счета 
    public class AddedScoreEvent
    {
        public int Count { get; private set; }

        public AddedScoreEvent(int count)
        {
            Count = count;
        }
    }

    // Ивент запуска игрового режима
    public class StartGameModeEvent
    {
        public GameModeState State { get; private set; }
        public bool IsContinue { get; private set; }

        public StartGameModeEvent(GameModeState state, bool isContinue)
        {
            State = state;
            IsContinue = isContinue;
        }
    }

    // Ивент завершения игрового режима
    public class EndGameModeEvent { }

    public class GameModeController : MonoBehaviour
    {
        public static bool IsPlaying { get; private set; } // Запущена ли игра

        private void Awake()
        {
            IsPlaying = false;
        }

        private void Start()
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null && gameModeState.Type != GameModeType.None) // Если есть прошлая игра, загружаем и продолжаем её
                StartGame(gameModeState.Type, false);

#if Yandex
            YG.YG2.GameReadyAPI();
#endif

#if GAME_PUSH
            GamePush.GP_Game.GameReady();
#endif
        }

        public void StartGame(GameModeType type, bool restart)
        {
            IsPlaying = true;

            var isContinue = true;
            var gameModeState = SaveManager.GameModeState;
            if (restart || gameModeState == null || gameModeState.Type != type) // Если рестарт или выбран другой режим игры от текущего то начинаем новую игру
            {
                isContinue = false;

                gameModeState = SaveManager.GetDefaultState<GameModeState>();
                gameModeState.Type = type;

                SaveManager.SetGameModeState(gameModeState);
            }

            if (type == GameModeType.Classic)
                gameModeState.ConnectionType = Assets.ConnectionType.Line5;

            AudioController.Play("start_game");
#if CRAZY_GAMES
            CrazySDK.Game.GameplayStart();
#endif

#if Poki
            PokiUnitySDK.Instance.gameplayStart();
#endif

            if (!TutorialController.IsCompleted(TutorialIds.FIRST_GAME)) // Начинаем туториал если он не завершен
            {
                TutorialController.StartTutorial(TutorialIds.FIRST_GAME, false);
                return;
            }

            EventsManager.Publish(new StartGameModeEvent(gameModeState, isContinue));

            SaveManager.Save(SaveType.GameMode);

            AnalyticsController.SendEvent(type == GameModeType.Classic ? "level_classic" : "level_adventure");
        }

        // Добавляем игровой счёт
        private void OnAddedScoreEvent(AddedScoreEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState == null)
                return;

            var balanceData = BalanceManager.Get(); // Берем текущий игровой баланс

            var oldScore = gameModeState.Score;
            gameModeState.Score += eventData.Count;

            // Если денег становится нужного количества чтобы увеличилась вместимость лавочек, то обновляем лавочки
            if (oldScore < balanceData.CoinsToIncreaseBenchesCapacity && gameModeState.Score >= balanceData.CoinsToIncreaseBenchesCapacity)
                EventsManager.Publish(new ReloadBenchesEvent());
        }

        private void OnEndGameMode(EndGameModeEvent eventData)
        {
            IsPlaying = false;

            var gameState = SaveManager.GameState;
            var gameModeState = SaveManager.GameModeState;

            EventsManager.Publish(new OpenPanelEvent(UIPanelType.Result)); // Показываем экран завершения игры

            SaveManager.Save(SaveType.GameMode);
            SaveManager.Sync();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<EndGameModeEvent>(OnEndGameMode);
            EventsManager.Subscribe<AddedScoreEvent>(OnAddedScoreEvent);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<EndGameModeEvent>(OnEndGameMode);
            EventsManager.Unsubscribe<AddedScoreEvent>(OnAddedScoreEvent);
        }
    }
}