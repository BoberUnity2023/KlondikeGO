using System.Collections.Generic;
using BloomLines.Managers;
using BloomLines.Tutorial;
using UnityEngine;

#if CONSOLE
using QFSW.QC;
#endif

namespace BloomLines.Controllers
{
    public class TutorialIds
    {
        public const string FIRST_GAME = "first_game";
    }

    public static class TutorialController 
    {
        private static Dictionary<string, TutorialBase> _tutorials;
        private static string _currentActiveTutorial;

        public static bool IsActive => !string.IsNullOrEmpty(_currentActiveTutorial); // Есть ли активное задание

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _tutorials = new Dictionary<string, TutorialBase>();
        }

        // Добавляем туториал
        public static void RegisterTutorial(TutorialBase tutorialBase)
        {
            if (!_tutorials.ContainsKey(tutorialBase.Id))
                _tutorials.Add(tutorialBase.Id, tutorialBase);
        }

        // Удаляем туториал
        public static void RemoveTutorial(TutorialBase tutorialBase)
        {
            if (_tutorials.ContainsKey(tutorialBase.Id))
                _tutorials.Remove(tutorialBase.Id);
        }

        // Запускаем туториал
        public static void StartTutorial(string tutorialId, bool permanent)
        {
            if(IsActive)
            {
                Debug.LogWarning($"Have active tutorial: {_currentActiveTutorial}");
                return;
            }

            if (!_tutorials.ContainsKey(tutorialId))
            {
                Debug.LogWarning($"Missing tutorial with id: {tutorialId}");
                return;
            }

            if(IsCompleted(tutorialId) && !permanent)
            {
                Debug.LogWarning($"Tutorial with id: '{tutorialId}' is already completed");
                return;
            }

            _currentActiveTutorial = tutorialId;

            var tutorialBase = _tutorials[tutorialId];
            tutorialBase.StartTutorial(OnCompleteTutorial);
        }

        // Туториал выполнен
        private static void OnCompleteTutorial(TutorialBase tutorialBase)
        {
            if (_currentActiveTutorial != tutorialBase.Id)
                return;

            if (IsCompleted(tutorialBase.Id))
                return;

            var gameState = SaveManager.GameState;
            gameState.CompletedTutorials.Add(tutorialBase.Id);

            SaveManager.Save(SaveType.Game); 
            SaveManager.Sync();
        }

        // Проверяем выполненно ли конкретный туториал
        public static bool IsCompleted(string tutorialId)
        {
            var gameState = SaveManager.GameState;
            if (gameState == null)
                return false;

            return gameState.CompletedTutorials.Contains(tutorialId);
        }

#if CONSOLE
        [Command("reset_all_tutorials")]
        private static void ResetAllTutorials()
        {
            var gameState = SaveManager.GameState;
            gameState.CompletedTutorials.Clear();

            SaveManager.Save(SaveType.Game);
        }

        [Command("complete_tutorial")]
        private static void CompleteTutorial(string tutorialId)
        {
            var gameState = SaveManager.GameState;
            gameState.CompletedTutorials.Add(tutorialId);

            SaveManager.Save(SaveType.Game);

            Debug.LogWarning("Restart Game");
        }
#endif
    }
}