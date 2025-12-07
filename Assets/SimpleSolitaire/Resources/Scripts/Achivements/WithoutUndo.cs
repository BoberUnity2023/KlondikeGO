namespace BloomLines
{
    public class WithoutUndo : AchivementBase
    {
        //Выиграть игру не делая отмены хода
        private bool _usedUndo;

        public override void Init()
        {
            base.Init();
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
            _usedUndo = false;
        }

        private void OnUndo()
        {
            _usedUndo = true;
        }

        private void OnGameWin()
        {
            if(!_usedUndo)
                StepAdd();
        }
    }
}
