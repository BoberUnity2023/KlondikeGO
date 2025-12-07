namespace BloomLines
{
    public class WinGamesToday : AchivementBase
    {
        //Разложить 30 партий за 1 день       

        public override void Init()
        {
            base.Init();            
            Hub.OnGameWin += OnGameWin;
            Hub.OnLastVisitNoToday += OnLastVisitNoToday;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;            
            Hub.OnLastVisitNoToday -= OnLastVisitNoToday;
        }        

        private void OnGameWin()
        {
            StepAdd();
        }

        private void OnLastVisitNoToday()
        {
            if (IsComplete)
                return;

            Progress = 0;
        }
    }
}
