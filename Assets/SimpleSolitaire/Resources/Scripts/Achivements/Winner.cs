public class Winner : AchivementBase
{
    //Выиграть 100 игр
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
