namespace BloomLines
{
    public class NoFuss : AchivementBase
    {
        //Пройти пасьянс менее чем за 20 минут
        private int _timeCount;
        public override void Init()
        {
            base.Init();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;
            Hub.OnTimeCount += OnTimeCount;
        }        

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart; 
            Hub.OnTimeCount -= OnTimeCount;
        }

        private void OnGameStart()
        {            
            _timeCount = 0;
        }        

        private void OnGameWin()
        {
            if (_timeCount < 1200)
                StepAdd();
        }

        private void OnTimeCount(int count)
        {
            _timeCount = count;
        }
    }
}
