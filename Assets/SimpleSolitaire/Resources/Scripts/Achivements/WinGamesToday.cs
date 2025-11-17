namespace BloomLines
{
    public class WinGamesToday : AchivementBase
    {
        //Разложить 30 партий за 1 день
        private bool _usedRotate;

        protected override void Start()
        {
            base.Start();            
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
