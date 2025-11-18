using UnityEngine;
using SimpleSolitaire.Controller;
using System.Linq;

public class AchivementsController: MonoBehaviour
{
    [SerializeField] private GameManager _hub;
    [SerializeField] private WindowAchivementInfo _windowInfo;
    [SerializeField] private AchivementBase[] _achivements;

    public int CompletedAchivements
    {
        get
        {
            return (from p in _achivements where p.IsComplete select p).Count();            
        }
    }

    private void Awake()
    {
        for (int i = 0; i < _achivements.Length; i++)
        {            
            _achivements[i].Id = i;
            _achivements[i].Hub = _hub;            
        }
    }

    public void PressInfo(int id)
    {
        _windowInfo.gameObject.SetActive(true);
        _hub.AppearWindow(_windowInfo.gameObject);        
        _windowInfo.SetTitle(_achivements[id].TitleKey);        
        _windowInfo.SetDescription(_achivements[id].DescriptionKey);
        _windowInfo.SetProgress(ProgressText(id));
        _windowInfo.SetIcon(_achivements[id].Icon);
    }

    public void PressCloseInfo()
    {
        _hub.DisappearWindow(_windowInfo.gameObject, OnDissapear);
        void OnDissapear()
        {
            _windowInfo.gameObject.SetActive(false);
        }
    }

    private string ProgressText(int id)
    {
        if (_achivements[id].IsComplete)
            return I2.Loc.LocalizationManager.GetTranslation("done");// "Выполнено";

        if (_achivements[id].TargetProgress == 1)
            return "";

        return _achivements[id].Progress.ToString() + "/" + _achivements[id].TargetProgress.ToString();
    }
}
