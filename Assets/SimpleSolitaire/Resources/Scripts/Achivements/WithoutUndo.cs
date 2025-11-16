namespace BloomLines
{
    public class WithoutUndo : AchivementBase
    {
        //Выиграть игру не делая отмены хода
        private int _countUndo;

        protected override void Start()
        {
            base.Start();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;
            Hub.OnUndo += OnUndo;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart;
            Hub.OnUndo -= OnUndo;
        }

        private void OnGameStart()
        {
            _countUndo = 0;
        }

        private void OnUndo()
        {
            _countUndo++;
        }

        private void OnGameWin()
        {
            if(_countUndo == 0)
                StepAdd();
        }
    }
}
