using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloomLines.Assets;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.Tasks;
using BloomLines.UI;
using UnityEngine;

namespace BloomLines.Controllers
{
    public enum TaskType
    {
        CollectFlowers,
        CollectProducts,
    }

    public enum TaskStateType
    {
        Unactive,
        Taken,
        Started,
    }

    public class LoseTaskEvent { } // Ивент когда проиграли задание

    public class TaskController : MonoBehaviour
    {
        [SerializeField] private UITaskTab _taskTab;

        private TaskBase _currentTask;

        // Закрыть задание и спрятать его
        private void CloseTask()
        {
            var gameModeState = SaveManager.GameModeState;
            gameModeState.TaskState = null;

            if (_currentTask != null)
            {
                Destroy(_currentTask);
                _currentTask = null;

                _taskTab.Hide();

                gameModeState.SellItemsCountAfterCompleteTask = 0;
                EventsManager.Publish(new SetConnectionTypeEvent(gameModeState.ConnectionType));
            }

            EventsManager.Publish(new ReloadBenchesEvent());
        }
        
        private TaskState GenerateNewTask()
        {
            var balanceData = BalanceManager.Get();
            var gameModeState = SaveManager.GameModeState;
            var stage = (gameModeState.CompletedTasksCount / balanceData.TasksCountInStage) + 1; // Берем текущий этап заданий
            stage = Mathf.Clamp(stage, 1, 10);

            var allStageTasks = GameAssets.TaskDatas.Values.Where(e => e.Stage == stage).ToList(); // Берем все заданий с данным этапом
            if(allStageTasks == null || allStageTasks.Count <= 0)
                return null;

            float totalChance = 0f;

            foreach (var task in allStageTasks)
                totalChance += task.Rarity;

            float currentSum = 0f;
            float randomValue = Random.Range(0f, totalChance);

            TaskData finalTaskData = null;
            foreach (var task in allStageTasks) // Берем рандомное из них исходя из них редкости спавна
            {
                currentSum += task.Rarity;
                if (randomValue <= currentSum)
                {
                    finalTaskData = task;
                    break;
                }
            }

            if (finalTaskData == null)
                finalTaskData = allStageTasks[0];

            var taskState = new TaskState(finalTaskData.Id);
            var itemsCount = finalTaskData.SlotsInBenches[Random.Range(0, finalTaskData.SlotsInBenches.Length)];

            bool isTimed = Random.Range(0f, 1f) < finalTaskData.ChanceToTimed; // Шанс задания на время

            taskState.Data += $"{itemsCount}"; // В данные задания записываем сколько предметов нужно собрать

            switch (finalTaskData.Type)
            {
                case TaskType.CollectFlowers: // Если задание на сбор цветов то больше данных для генерации не нужно
                    break;
                case TaskType.CollectProducts: // Если задание на сбор товаров

                    var connectionTypes = new List<ConnectionType>() // Все типы соединения
                    {
                        ConnectionType.Line5,
                        ConnectionType.Square,
                        ConnectionType.Triangle,
                        ConnectionType.Plus,
                    };

                    var productsTaskData = finalTaskData as CollectProductsTaskData;

                    taskState.Data += $";{productsTaskData.ProductsTypeCount}"; // В данные задания добавляем количество типов товаров

                    var itemsCountPerProduct = itemsCount / productsTaskData.ProductsTypeCount;
                    for(int i = 0; i < productsTaskData.ProductsTypeCount; i++) 
                    {
                        var currentItemsCount = Random.Range(1, itemsCountPerProduct + 1);
                        if (i >= productsTaskData.ProductsTypeCount - 1)
                            currentItemsCount = itemsCount;

                        var connectionType = connectionTypes[Random.Range(0, connectionTypes.Count)];
                        taskState.Data += $";{currentItemsCount}|{connectionType}"; // В данные задания добавляем тип соединения и количество товаров в данном типе

                        itemsCount -= currentItemsCount;

                        connectionTypes.Remove(connectionType);
                    }

                    break;
            }

            if (isTimed)
                taskState.Data += $";{finalTaskData.TimeInSeconds}"; // Если задание на время то в конен данных еще добавляем данные о оставшихся секундах

            if(!string.IsNullOrEmpty(taskState.Data))
                taskState.Data = taskState.Data.TrimStart(';');

            Debug.Log($"Generate Task: {taskState.Id} : {taskState.Data}");
            AnalyticsController.SendEvent("task_start");
            return taskState;

            // Итого данны задания должны быть в таком формате:
            // Задание на сбор цветов:
            // Data = "количество цветов для сбора"
            // Задание на сбор товаров:
            // Data = "количество товаров для сбора;количество товаров соединения1;тип соединения1" дальше могут добавляться соединения 2 3 и т.д по такой же логике
            // Если задание на время в конце ко всем даннім добавляется Data += ";количество секунд"
        }

        // Пытаемся показать задание
        private void TryShowTask(bool isNew)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.Type != GameModeType.Adventure) // Если не режим приключения
                return;

            if (isNew && gameModeState.MovesCountAfterSell > 0) // Если попытка показать новое задание но уже сделан ход 
                return;

            if (gameModeState.TaskState == null || string.IsNullOrEmpty(gameModeState.TaskState.Id)) // Если нет задания генерируем новое
                gameModeState.TaskState = GenerateNewTask();

            var taskData = GameAssets.GetTaskData(gameModeState.TaskState.Id);
            switch (taskData.Type)
            {
                case TaskType.CollectFlowers:
                    _currentTask = gameObject.AddComponent<CollectFlowersTask>(); // Добавляем задание на сбор цветов
                    break;
                case TaskType.CollectProducts:
                    _currentTask = gameObject.AddComponent<CollectProductsTask>(); // Добавляем задание на сбор товаров
                    break;
            }

            if(isNew)
                AudioController.Play("new_task");

            _currentTask.Initialize(gameModeState.TaskState);
            _taskTab.Show(gameModeState.TaskState, _currentTask);
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            if (!eventData.IsContinue) // Если новая игра прячем задания
                CloseTask();

            var state = eventData.State;
            if (state.TaskState != null && !string.IsNullOrEmpty(state.TaskState.Id)) // Если есть сохраненное задание, отображаем его
                TryShowTask(false);
        }

        // Когда сделали ход
        private void OnMakeMove(MakeMoveEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.TaskState != null) // Если есть задание
            {
                if (gameModeState.TaskState.StateType == TaskStateType.Unactive) // Если задание не взяли то закрываем его
                    CloseTask();
                else if(gameModeState.TaskState.StateType == TaskStateType.Taken) // Если задание взяли то стартуем его
                    _taskTab.StartTask();
            }
        }

        // Продаем товары с лавочек
        private void OnSellItems(SellItemsEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (_currentTask != null && gameModeState.TaskState != null) // Если есть активное задание
            {
                _currentTask.OnSellItems(eventData);

                if (_currentTask.IsCompleted()) // Если задание выполненно
                {
                    int reward = gameModeState.TaskState.GetReward();

                    Vibration.Vibrate(70);
                    AudioController.Play("complete_task");

                    Debug.Log("Task completed, reward: " + reward);

                    gameModeState.CompletedTasksCount++;

                    EventsManager.Publish(new AddedScoreEvent(reward));
                    EventsManager.Publish(new ShowNotificationEvent("task_completed", reward.ToString()));

                    CloseTask();
                    AnalyticsController.SendEvent("task_completed");
                }
                else
                {
                    _taskTab.UpdateVisual();

                    var currentGoalInfo = _currentTask.GetCurrentGoalInfo();
                    EventsManager.Publish(new SetConnectionTypeEvent(currentGoalInfo.ConnectionType));
                }
            }
            else
            {
                gameModeState.SellItemsCountAfterCompleteTask++; // Если активного задания нет и продали товары
                if(gameModeState.SellItemsCountAfterCompleteTask >= 1) // Если продали товары 1 раз после прошлого задания то спавним новое
                    TryShowTask(true);
            }
        }

        private void OnLoseTask(LoseTaskEvent eventData)
        {
            AudioController.Play("lose_task");

            EventsManager.Publish(new SellAllItemsFromBenchesEvent());

            CloseTask();

            AnalyticsController.SendEvent("task_lose");
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<LoseTaskEvent>(OnLoseTask);
            EventsManager.Subscribe<MakeMoveEvent>(OnMakeMove);
            EventsManager.Subscribe<SellItemsEvent>(OnSellItems);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<LoseTaskEvent>(OnLoseTask);
            EventsManager.Unsubscribe<MakeMoveEvent>(OnMakeMove);
            EventsManager.Unsubscribe<SellItemsEvent>(OnSellItems);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
        }
    }
}