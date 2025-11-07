using BloomLines.ObjectPool;

namespace BloomLines.Managers
{
    public static class SpawnerManager
    {
        public static BoardObjectsSpawner BoardObjectsSpawner { get; private set; }
        public static CutPlantSpawner CutPlantSpawner { get; private set; }
        public static SellItemSpawner SellItemSpawner { get; private set; }

        public static void RegisterSellItemSpawner(SellItemSpawner spawner)
        {
            SellItemSpawner = spawner;
        }

        public static void RegisterBoardObjectsSpawner(BoardObjectsSpawner spawner)
        {
            BoardObjectsSpawner = spawner;
        }

        public static void RegisterCutPlantSpawner(CutPlantSpawner spawner)
        {
            CutPlantSpawner = spawner;
        }
    }
}