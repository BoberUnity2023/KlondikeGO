using SimpleSolitaire.Controller;
using UnityEngine;
using UnityEngine.UI;

public class TabProgress : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Text _experienceIndicator;
    [SerializeField] private int[] _needExperience;
    [SerializeField] private ProgressLine[] _progressLines;
    //[SerializeField] private LBPlayer _player;

    private void OnEnable()
    {
        SetExperience(_gameManager.Stats.Experience);
        SetThisPlayerInfo();
    }


    public void SetExperience(int value)
    {
        int level = Mathf.Min(_needExperience.Length - 1, Level);
        Debug.Log("Exp : " + level);
        _experienceIndicator.text = value.ToString() + " /" + _needExperience[level].ToString();
        SetProgressLines(level);
    }

    private int Level
    {
        get
        {
            for (int i = _needExperience.Length - 1; i >= 0; i--)
            {
                if (_gameManager.Stats.Experience >= _needExperience[i])
                    return i + 1;
            }

            return 0;
        }
    }

    private void SetProgressLines(int level)
    {
        for (int i = 0; i < _progressLines.Length; i++)
        {
            _progressLines[i].SetAlpha(i == level ? 1 : 0.5f);
            //_progressLines[i].SetTitleColor(i < level ? Color.gray : new Color(1, 0.8f, 0, 1));
            _progressLines[i].SetMedalShiny(i == level);

            if (i <= level)
                _progressLines[i].IconShow();
            else
                _progressLines[i].IconHide();
        }
    }

    private void SetThisPlayerInfo()
    {

    }
}

