namespace BloomLines
{
    public class FirstStep : AchivementBase
    {
        //Пройти первую игру
        protected override void Start()
        {
            base.Start();
            Hub.OnGameWin += OnGameWin;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
        }

        private void OnGameWin()
        {
            StepAdd();
        }
    }
}
