using BloomLines.Assets;

namespace BloomLines.Tools
{
    public class SecateursTool : CrystalRequiredTool
    {
        public override void Apply(BalanceData data)
        {
            if(IsCollected)
                data.GeneratePlantsCountRange.y++;
        }
    }
}