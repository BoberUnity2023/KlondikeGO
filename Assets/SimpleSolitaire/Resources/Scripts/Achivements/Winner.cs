public class Winner : AchivementBase
{
    protected override void Start()
    {  
        base.Start();
        //Hub.Logic.OnGameWin += OnGameWin;
    }

    private void OnDestroy()
    {
        //Hub.Logic.OnGameWin -= OnGameWin;
    }

    private void OnGameWin()
    {
        StepAdd();
    }
}
