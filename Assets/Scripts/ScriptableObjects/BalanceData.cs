using System.Text;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "BalanceData", menuName = "BloomLines/BalanceData")]
    public class BalanceData : ScriptableObject
    {
        [SerializeField] public Vector2Int GeneratePlantsCountRange;
        [SerializeField] public int WebHealth;
        [SerializeField] public int TasksCountInStage;
        [SerializeField] public int CoinsToIncreaseBenchesCapacity;
        [SerializeField] public float SecondsToInterstitial;

        [HideInInspector] public float ChanceToSpawnRock;
        [HideInInspector] public float ChanceToSpawnWeb;
        [HideInInspector] public float ChanceToSpawnMole;

        public int GetTotalItemsCountInBenches(ConnectionType type)
        {
            var gameModeState = SaveManager.GameModeState;

            int totalItemsCount = 32;
            if (type != ConnectionType.Line4)
                totalItemsCount = 8;

            if(gameModeState.Score >= CoinsToIncreaseBenchesCapacity)
                totalItemsCount *= 2;

            if(gameModeState != null && gameModeState.TaskState != null && !string.IsNullOrEmpty(gameModeState.TaskState.Data))
            {
                var values = gameModeState.TaskState.Data.Split(";");
                int itemsCount = int.Parse(values[0]);
                totalItemsCount = itemsCount;
            }

            return totalItemsCount;
        }

        public BalanceData Clone()
        {
            return (BalanceData)this.MemberwiseClone();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var gameModeState = SaveManager.GameModeState;

            sb.AppendLine(string.Empty);
            sb.AppendLine("BALANCE DATA");
            sb.AppendLine("---------------------------------------");
            sb.AppendLine($"WebHealth: {WebHealth}");
            sb.AppendLine($"GeneratePlantsCount: {GeneratePlantsCountRange.x}-{GeneratePlantsCountRange.y}");
            sb.AppendLine($"ChanceToSpawnRock: {Mathf.RoundToInt((ChanceToSpawnRock * 100f))}%");
            sb.AppendLine($"ChanceToSpawnWeb: {Mathf.RoundToInt((ChanceToSpawnWeb * 100f))}%");
            sb.AppendLine($"ChanceToSpawnMole: {Mathf.RoundToInt((ChanceToSpawnMole * 100f))}%");
            sb.AppendLine($"CoinsToIncreaseBenchesCapacity: {CoinsToIncreaseBenchesCapacity}");
            sb.AppendLine($"CurrentTotalItemInBenches: {(gameModeState == null ? 0 : (GetTotalItemsCountInBenches(gameModeState.ConnectionType)))}");

            return sb.ToString();
        }
    }
}