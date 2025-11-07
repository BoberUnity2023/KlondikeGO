using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.UI;

namespace BloomLines.Saving
{
    [System.Serializable] // Всё обьекты что должны сохраняться, должны наследоваться от этого класса
    public class SaveState
    {
        public string SaveVersion;
        public long SaveTime;
    }

    [System.Serializable] // Основное игровое сохранение
    public class GameState : SaveState
    {
        public LanguageCode LanguageCode;
        public string SkinPack; // Текущий скин пак
        public float MusicVolume;
        public float SoundVolume;
        public bool Vibration;
        public long LastDailyRewardTimestamp;
        public bool IsRateGame; // Всплывало ли уведомление оценить игру
        public List<string> Purchased = new List<string>(); // Внутриигровые покупки
        public PlayerStatsState Stats = new PlayerStatsState();
        public List<string> CompletedTutorials = new List<string>();
    }

    [System.Serializable]
    public class PlayerStatsState
    {
        public int MaxScore;
    }

    [System.Serializable] // Сохранение уровня
    public class GameModeState : SaveState
    {
        public int Score;
        public int MovesCountAfterSell; // Сколько ходов игрок сделал после продажи товаров
        public int MovesAfterSelectConnectionType; // Сколько ходов игрок сделал после того как сменил тип соединения
        public int ContinueGameCount; // Сколько раз игрок возродился
        public int TotalSellCount; // Сколько раз отправили товары на продажу
        public int MovesWithoutMoves; // Сколько раз ходов поле заросло
        public GameModeType Type;
        public ConnectionType ConnectionType;
        public BoardState BoardState;
        public List<ToolState> ToolStates;
        public TaskState TaskState;
        public int CompletedTasksCount;
        public int SellItemsCountAfterCompleteTask; // Сколько раз игрок продал товары после выполнения задания
        public List<string> FlowersInHouse = new List<string>();
        public List<string> BenchItems = new List<string>();

        public ToolState GetToolState(ToolType type)
        {
            if (type == ToolType.None)
                return null;

            if (ToolStates == null)
                ToolStates = new List<ToolState>();

            var toolState = ToolStates.FirstOrDefault(e => e.ToolType == type);
            if(toolState == null)
            {
                toolState = new ToolState();
                toolState.ToolType = type;
                ToolStates.Add(toolState);
            }

            return toolState;
        }
    }

    [System.Serializable]
    public class TaskState
    {
        public string Id;
        public TaskStateType StateType;
        public string Data;

        public int GetReward()
        {
            var taskData = GameAssets.TaskDatas.FirstOrDefault(e => e.Value.Id == Id).Value;
            bool isTimed = IsTimed();

            int reward = taskData.Price + (isTimed ? taskData.TimeAdditionalPrice : 0);

            return reward;
        }

        public int GetGoalsCount()
        {
            if (string.IsNullOrEmpty(Data))
                return 0;

            var taskData = GameAssets.GetTaskData(Id);
            if (taskData as CollectFlowersTaskData)
            {
                return 1;
            }
            else
            {
                var values = Data.Split(';');
                int connectionCount = IsTimed() ? values.Length - 3 : values.Length - 2;
                return connectionCount;
            }
        }

        public bool IsTimed()
        {
            return GetTime() > 0f;
        }

        // Время всегда в конце данных записывается, поэтому проверяем есть ли оно
        public float GetTime()
        {
            if (string.IsNullOrEmpty(Data))
                return 0f;

            var values = Data.Split(';');
            if (values.Length > 1 && float.TryParse(values[values.Length - 1], out float result))
                return result;

            return 0f;
        }

        public TaskState(string id)
        {
            Id = id;
        }
    }

    [System.Serializable]
    public class BoardState
    {
        public TileState[] Tiles;

        public BoardState()
        {
            Tiles = new TileState[Board.WIDTH * Board.HEIGHT];
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new TileState();
        }
    }

    [System.Serializable]
    public class TileState
    {
        public TileObjectState ObjectState;

        public bool HaveObject()
        {
            return ObjectState != null && !string.IsNullOrEmpty(ObjectState.Id);
        }
    }

    [System.Serializable]
    public class TileObjectState
    {
        public string Id;
        public string Data;
    }

    [System.Serializable]
    public class ToolState
    {
        public ToolType ToolType;
        public string Data;
    }
}