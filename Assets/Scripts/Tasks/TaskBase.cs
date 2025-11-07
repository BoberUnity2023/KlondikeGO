using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Saving;
using NUnit.Framework;
using UnityEngine;

namespace BloomLines.Tasks
{
    public class TaskGoalInfo
    {
        public ConnectionType ConnectionType { get; private set; }
        public int TargetValue { get; private set; }

        public TaskGoalInfo(ConnectionType connectionType, int targetValue)
        {
            ConnectionType = connectionType;
            TargetValue = targetValue;
        }
    }

    public abstract class TaskBase : MonoBehaviour
    {
        protected TaskState _state;

        public virtual void Initialize(TaskState state)
        {
            _state = state;
        }

        public abstract bool IsCompleted();

        public abstract TaskGoalInfo[]  GetAllGoalsInfo();

        public abstract TaskGoalInfo GetCurrentGoalInfo();

        public abstract void OnCollectItemToBench(CollectItemToBenchEvent eventData);

        public abstract void OnSellItems(SellItemsEvent eventData);
    }
}