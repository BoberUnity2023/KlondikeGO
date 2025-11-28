
namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        public bool NoAds;
        public int Score;
        public int Gold;
        public int Experience;
        public int PlayedGames;
        public int Wins;
        public int Losts;
        public int FastestWinTime = 7200;
        public int FastestPartyTime = 7200;
        public int LongestPartyTime;
        public string LastVisitTime;
        public bool[] TakenDayBonuses = new bool[5];
        public int[] AchivementProgress = new int[24];
        public bool[] CardBacks = new bool[60];
        public bool[] Cards = new bool[20];
        public bool[] Backgrounds = new bool[25];
    }
}
