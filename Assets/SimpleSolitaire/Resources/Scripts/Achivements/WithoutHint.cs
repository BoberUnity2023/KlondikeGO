namespace BloomLines
{
    public class WithoutHint : AchivementBase
    {
        //–азложить пась€нс без использовани€ подсказок
        private bool _usedHint;

        protected override void Start()
        {
            base.Start();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;
            Hub.OnUndo += OnHint;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart;
            Hub.OnUndo -= OnHint;
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
