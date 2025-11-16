using UnityEngine;
using UnityEngine.UI;

public class WindowAchivementInfo : MonoBehaviour
{
    [SerializeField] private Text _title;
    [SerializeField] private Text _description;
    [SerializeField] private Text _progress;
    [SerializeField] private Image _icon;    

    public void SetTitle(string key)
    {        
        string text = I2.Loc.LocalizationManager.GetTranslation(key);
        _title.text = text;
    }

    public void SetDescription(string key)
    {
        string text = I2.Loc.LocalizationManager.GetTranslation(key);
        _description.text = text;
    }

    public void SetProgress(string text)
    {
        _progress.text = text;
    }

    public void SetIcon(Sprite icon)
    {
        _icon.sprite = icon;
    }
}
