namespace BloomLines
{
    public class FirstStep : AchivementBase
    {
        //Пройти первую игру
        public override void Init()
        {
            base.Init();
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
