using GameAnalyticsSDK;
using SimpleSolitaire.Controller;
using UnityEngine;

public class AchivementBase : MonoBehaviour
{
    [SerializeField] private AchivementUI _achivementUI;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _title;
    [SerializeField] private string _description;
    [SerializeField] private int _targetProgress;

    public int Id { get; set;}
    public GameManager Hub { get; set; }

    public string Description => _description;
    public int Progress 
    {
        get 
        {
            return  Hub.Stats.GetAchivementProgress(Id);
        }
        set
        {
            Hub.Stats.SetAchivementProgress(Id, value);
        }
    }

    public int TargetProgress => _targetProgress;

    public string Title => _title;  

    public Sprite Icon => _icon;

    protected virtual void Start()
    {
        if (IsComplete)
            _achivementUI.ShowComplete();
        else
            _achivementUI.ShowNoComplete();
    }

    public bool IsComplete
    {
        get { return Progress >= _targetProgress; }
    }

    public void StepAdd()
    {
        if (IsComplete)
            return;

        Progress++;
        if (Progress == _targetProgress)
            Complete();
    }

    public void Complete()
    {
        Debug.Log("јчивка выполнена! " + _description);
        _achivementUI.ShowComplete();
        Hub.AudioController.Play(AudioController.AudioType.Bonus);
        GameAnalytics.NewDesignEvent("AchivementComplete: " + Title);        
    }
}
