using BloomLines.Assets;

namespace BloomLines.Tools
{
    public class RakeTool : CrystalRequiredTool
    {
        public override void Apply(BalanceData data)
        {
            if (IsCollected)
                data.GeneratePlantsCountRange.x++;
        }
    }
}