namespace BloomLines
{
    public class WithoutHint : AchivementBase
    {
        //–азложить пась€нс без использовани€ подсказок
        private bool _usedHint;

        public override void Init()
        {
            base.Init();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;
            Hub.OnHint += OnHint;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart;
            Hub.OnHint -= OnHint;
        }

        private void OnGameStart()
        {
            _usedHint = false;
        }

        private void OnHint()
        {
            _usedHint = true;
        }

        private void OnGameWin()
        {
            if(!_usedHint)
                StepAdd();
        }
    }
}
