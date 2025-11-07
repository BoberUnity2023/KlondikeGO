using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BloomLines.Controllers;
using System;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BloomLines.Assets
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class GameAssets
    {
#if UNITY_EDITOR
        static GameAssets()
        {
            Initialize();
        }
#endif

        public static BalanceData BalanceData { get; private set; }
        public static Dictionary<string, SkinPackData> SkinPackDatas { get; private set; }
        public static Dictionary<string, NotificationData> NotificationDatas { get; private set; }
        public static Dictionary<ToolType, ToolData> ToolDatas { get; private set; }
        public static Dictionary<ConnectionType, ConnectionTypeData> ConnectionTypeDatas { get; private set; }
        public static Dictionary<string, BoardObjectData> BoardObjectDatas { get; private set; }
        public static Dictionary<string, SellItemData> SellItemDatas { get; private set; }
        public static Dictionary<string, TaskData> TaskDatas { get; private set; }
        public static List<DailyRewardData> DailyRewardDatas { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            BalanceData = Resources.Load<BalanceData>("BalanceData");

            #region ToolDatas
            ToolDatas = new Dictionary<ToolType, ToolData>();
            var toolDatas = Resources.LoadAll<ToolData>("Data/Tools");
            foreach(var data in toolDatas)
            {
                if(!ToolDatas.ContainsKey(data.ToolType))
                    ToolDatas.Add(data.ToolType, data);
            }
            #endregion

            #region ConnectionTypeDatas
            ConnectionTypeDatas = new Dictionary<ConnectionType, ConnectionTypeData>();
            var connectionTypeDatas = Resources.LoadAll<ConnectionTypeData>("Data/ConnectionTypes");
            foreach (var data in connectionTypeDatas)
            {
                if (!ConnectionTypeDatas.ContainsKey(data.ConnectionType))
                    ConnectionTypeDatas.Add(data.ConnectionType, data);
            }
            #endregion

            #region BoardObjectDatas
            BoardObjectDatas = new Dictionary<string, BoardObjectData>();
            var boardObjectDatas = Resources.LoadAll<BoardObjectData>("Data/BoardObjects");
            foreach (var data in boardObjectDatas)
            {
                if (!BoardObjectDatas.ContainsKey(data.Id))
                    BoardObjectDatas.Add(data.Id, data);
            }
            #endregion

            #region SellItemDatas
            SellItemDatas = new Dictionary<string, SellItemData>();
            var sellItemDatas = Resources.LoadAll<SellItemData>("Data/SellItems");
            foreach(var data in sellItemDatas)
            {
                if (!SellItemDatas.ContainsKey(data.Id))
                    SellItemDatas.Add(data.Id, data);
            }
            #endregion

            #region NotificationDatas
            NotificationDatas = new Dictionary<string, NotificationData>();
            var notificationsDatas = Resources.LoadAll<NotificationData>("Data/Notifications");
            foreach(var data in notificationsDatas)
            {
                if(!NotificationDatas.ContainsKey(data.Id))
                    NotificationDatas.Add(data.Id, data);
            }
            #endregion

            #region SkinPackDatas
            SkinPackDatas = new Dictionary<string, SkinPackData>();
            var skinPackDatas = Resources.LoadAll<SkinPackData>("Data/SkinPacks");
            foreach(var data in skinPackDatas)
            {
                if (!SkinPackDatas.ContainsKey(data.Id))
                    SkinPackDatas.Add(data.Id, data);
            }
            #endregion

            #region TaskDatas
            TaskDatas = new Dictionary<string, TaskData>();
            var taskDatas = Resources.LoadAll<TaskData>("Data/Tasks");
            foreach(var data in taskDatas)
            {
                if (!TaskDatas.ContainsKey(data.Id))
                    TaskDatas.Add(data.Id, data);
            }
            #endregion

            #region DailyRewardDatas
            DailyRewardDatas = Resources.LoadAll<DailyRewardData>("Data/DailyRewards").ToList();
            #endregion
        }

        public static BoardObjectData GetRandomBoardObjectData()
        {
            return BoardObjectDatas.Values.ElementAt(Random.Range(0, BoardObjectDatas.Values.Count));
        }

        public static ToolData GetToolData(ToolType type)
        {
            return ToolDatas[type];
        }

        public static TaskData GetTaskData(string id)
        {
            if(TaskDatas.ContainsKey(id))
                return TaskDatas[id];

            return null;
        }

        public static SkinPackData GetSkinPackData(string id)
        {
            if(SkinPackDatas.ContainsKey(id))
                return SkinPackDatas[id];

            return null;
        }

        public static BoardObjectData GetBoardObjectData(string id)
        {
            if (BoardObjectDatas.ContainsKey(id))
                return BoardObjectDatas[id];

            return null;
        }

        public static NotificationData GetNotificationData(string id)
        {
            if(NotificationDatas.ContainsKey(id))
                return NotificationDatas[id];

            return null;
        }

        public static SellItemData GetSellItemData(string id)
        {
            if (SellItemDatas.ContainsKey(id))
                return SellItemDatas[id];

            return null;
        }

        public static SellItemData GetSellItemData(ConnectionType connectinType, PlantType plantType)
        {
            var itemData = SellItemDatas.FirstOrDefault(e => e.Value.ConnectionType == connectinType && e.Value.PlantType == plantType);
            return itemData.Value;
        }

        public static ConnectionTypeData GetConnectionTypeData(ConnectionType connectionType)
        {
            if (ConnectionTypeDatas.ContainsKey(connectionType))
                return ConnectionTypeDatas[connectionType];

            return null;
        }

        public static DailyRewardData GetDailyRewardData(GameModeType gameModeType, DayOfWeek dayOfWeek)
        {
            var data = DailyRewardDatas.FirstOrDefault(e => e.GameModeType == gameModeType && e.DayOfWeek == dayOfWeek);
            return data;
        }
    }
}