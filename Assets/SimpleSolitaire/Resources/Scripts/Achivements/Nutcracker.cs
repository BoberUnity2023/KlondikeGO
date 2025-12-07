namespace BloomLines
{
    public class Nutcracker : AchivementBase
    {
        //Сделать 500 кликов за 1 игру        

        public override void Init()
        {
            base.Init();
            Hub.OnGameStart += OnGameStart;
            Hub.OnGameWin += OnGameWin;
            Hub.OnClick += OnClick;            
        }

        private void OnDestroy()
        {
            Hub.OnGameStart -= OnGameStart;
            Hub.OnGameWin -= OnGameWin;            
            Hub.OnClick -= OnClick;
        }

        private void OnGameStart()
        {
            if (!IsComplete) 
                Progress = 0;
        }

        private void OnGameWin()
        {
            if (!IsComplete)
                Progress = 0;
        }

        private void OnClick()
        {
            StepAdd();
        }
    }
}
