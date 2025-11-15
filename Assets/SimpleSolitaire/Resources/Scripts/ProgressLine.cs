using UnityEngine;
using UnityEngine.UI;

public class ProgressLine : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Text _title;
    [SerializeField] private GameObject _star;
    [SerializeField] private GameObject _medal;

    public void SetTitleColor(Color color)
    {
        _title.color = color;
    }

    public void SetAlpha(float value)
    {
        _canvasGroup.alpha = value;
    }

    public void IconShow()
    {
        _star.SetActive(true);
        _medal.SetActive(true);
    }

    public void IconHide()
    {
        _star.SetActive(false);
        _medal.SetActive(false);        
    }
}
