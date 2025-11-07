using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;

namespace BloomLines.Tasks
{
    public class CollectFlowersTask : TaskBase
    {
        private bool _isCompleted;

        public override void Initialize(TaskState state)
        {
            base.Initialize(state);

            _isCompleted = false;
        }

        public override TaskGoalInfo[] GetAllGoalsInfo()
        {
            var result = new TaskGoalInfo[1];
            result[0] = GetCurrentGoalInfo();

            return result;
        }

        public override TaskGoalInfo GetCurrentGoalInfo()
        {
            var balanceData = GameAssets.BalanceData;
            var gameModeState = SaveManager.GameModeState;
            var taskData = GameAssets.GetTaskData(_state.Id);

            var values = _state.Data.Split(";");
            var itemsCount = int.Parse(values[0]) - gameModeState.BenchItems.Count;

            return new TaskGoalInfo(ConnectionType.Line4, itemsCount);
        }

        public override bool IsCompleted()
        {
            return _isCompleted;
        }

        public override void OnSellItems(SellItemsEvent eventData)
        {
            _isCompleted = true;
        }

        public override void OnCollectItemToBench(CollectItemToBenchEvent eventData)
        {
        }
    }
}