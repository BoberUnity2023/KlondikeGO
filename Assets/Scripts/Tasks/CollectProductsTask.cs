using System;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Saving;

namespace BloomLines.Tasks
{
    public class CollectProductsTask : TaskBase
    {
        private bool _isCompleted;

        public override void Initialize(TaskState state)
        {
            base.Initialize(state);

            _isCompleted = false;
        }

        public override TaskGoalInfo[] GetAllGoalsInfo()
        {
            var taskData = GameAssets.GetTaskData(_state.Id);
            var balanceData = GameAssets.BalanceData;

            if (string.IsNullOrEmpty(_state.Data))
                return null;

            var values = _state.Data.Split(';');
            var isTimed = _state.IsTimed();
            int goalsCount = _state.GetGoalsCount();

            var result = new TaskGoalInfo[goalsCount];

            for (int i = 2; i < values.Length - (isTimed ? 1 : 0); i++)
            {
                var connectionTypeValues = values[i].Split("|");
                var connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connectionTypeValues[1], true);
                int itemsCount = int.Parse(connectionTypeValues[0]);

                result[i - 2] = new TaskGoalInfo(connectionType, itemsCount);
            }

            return result;
        }

        public override TaskGoalInfo GetCurrentGoalInfo()
        {
            var taskData = GameAssets.GetTaskData(_state.Id);
            var balanceData = GameAssets.BalanceData;

            var values = _state.Data.Split(';');
            if (values.Length >= 3)
            {
                var currentTypeValues = values[2].Split("|");

                var connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), currentTypeValues[1], true);
                var itemsCount = int.Parse(currentTypeValues[0]);

                return new TaskGoalInfo(connectionType, itemsCount);
            }


            return null;
        }

        public override bool IsCompleted()
        {
            return _isCompleted;
        }

        public override void OnCollectItemToBench(CollectItemToBenchEvent eventData)
        {
            if (_state == null || string.IsNullOrEmpty(_state.Data))
                return;

            var values = _state.Data.Split(';');
            _state.Data = string.Empty;

            if (values[2].Contains("|"))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (i == 2)
                    {
                        var connectionTypeValues = values[i].Split("|");
                        var count = int.Parse(connectionTypeValues[0]) - 1;
                        if(count > 0)
                            _state.Data += $";{count}|{connectionTypeValues[1]}";
                    }
                    else
                    {
                        _state.Data += $";{values[i]}";
                    }
                }

                _state.Data = _state.Data.TrimStart(';');
            }
        }

        public override void OnSellItems(SellItemsEvent eventData)
        {
            _isCompleted = true;
        }
    }
}