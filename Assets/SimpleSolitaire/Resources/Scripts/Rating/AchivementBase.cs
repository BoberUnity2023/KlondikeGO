using GameAnalyticsSDK;
using SimpleSolitaire.Controller;
using UnityEngine;

public class AchivementBase : MonoBehaviour
{
    [SerializeField] private AchivementUI _achivementUI;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _titleKey;
    [SerializeField] private string _descriptionKey;
    [SerializeField] private int _targetProgress;
    [SerializeField] private bool _saved;
    private int _progress;

    public int Id { get; set;}
    public GameManager Hub { get; set; }

    public string DescriptionKey => _descriptionKey;
    public int Progress 
    {
        get 
        {
            if (_saved)
                return  Hub.Stats.GetAchivementProgress(Id);
            
            return _progress;
        }
        set
        {
            if (IsComplete)
                return;

            _progress = value;

            if (_saved || _progress >= _targetProgress) 
                Hub.Stats.SetAchivementProgress(Id, value);
        }
    }

    public int TargetProgress => _targetProgress;

    public string TitleKey => _titleKey;  

    public Sprite Icon => _icon;

    protected virtual void Start()
    {
        if (!_saved)
            _progress = Hub.Stats.GetAchivementProgress(Id);

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
        Debug.Log("јчивка выполнена! " + _descriptionKey);
        _achivementUI.ShowComplete();
        Hub.AudioController.Play(AudioController.AudioType.Bonus);
        GameAnalytics.NewDesignEvent("AchivementComplete: " + TitleKey);        
    }
}
