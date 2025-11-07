using BloomLines.Assets;

namespace BloomLines.Tools
{
    public class PitchforkTool : CrystalRequiredTool
    {
        public override void Apply(BalanceData data)
        {
            if (IsCollected)
                data.GeneratePlantsCountRange.x++;
        }
    }
}