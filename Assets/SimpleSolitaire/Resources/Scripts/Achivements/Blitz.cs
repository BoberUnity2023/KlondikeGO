namespace BloomLines
{
    public class Blitz : AchivementBase
    {
        //Пройти игру менее чем за 3 минуты
        private int _timeCount;
        protected override void Start()
        {
            base.Start();
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
            if (_timeCount < 180)
                StepAdd();
        }

        private void OnTimeCount(int count)
        {
            _timeCount = count;
        }
    }
}
