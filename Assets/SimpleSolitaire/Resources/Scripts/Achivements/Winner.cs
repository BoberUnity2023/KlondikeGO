public class Winner : AchivementBase
{
    //Выиграть 100 игр
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
