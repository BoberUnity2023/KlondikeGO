using UnityEngine;
using SimpleSolitaire.Controller;

public class AchivementsController: MonoBehaviour
{
    [SerializeField] private GameManager _hub;
    [SerializeField] private WindowAchivementInfo _windowInfo;
    [SerializeField] private AchivementBase[] _achivements; 

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
        _windowInfo.SetTitle(_achivements[id].Title);
        _windowInfo.SetDescription(_achivements[id].Description);
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
            return "Выполнено";

        if (_achivements[id].TargetProgress == 1)
            return "";

        return _achivements[id].Progress.ToString() + "/" + _achivements[id].TargetProgress.ToString();
    }
}
