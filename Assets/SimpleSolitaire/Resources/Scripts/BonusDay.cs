using UnityEngine;
using UnityEngine.UI;

public class BonusDay : MonoBehaviour
{    
    [SerializeField] private GameObject _dayLast;
    [SerializeField] private Image _boxOpenedLastImage;
    [SerializeField] private GameObject _dayCurrent;
    [SerializeField] private Image _boxOpenedCurrentImage;
    [SerializeField] private GameObject _dayNext;
    [SerializeField] private Image _boxOpenedNextImage;
    [SerializeField] private Text _title;
    [SerializeField] private Text _dayIndicator;
    [SerializeField] private Text _price;
    [SerializeField] private Image _ok;
    [SerializeField] private Image _coin;
    [SerializeField] private Color _currentColor;
    [SerializeField] private Sprite[] _iconsOpened;
    [SerializeField] private Sprite[] _iconsClosed;

    public void SetLast(int day, int price)
    {
        _dayLast.SetActive(true);
        _dayCurrent.SetActive(false);
        _dayNext.SetActive(false);
        _ok.gameObject.SetActive(true);
        SetColor(Color.white);
        _dayIndicator.text = (day + 1).ToString();
        _price.text = price.ToString();
        _boxOpenedLastImage.sprite = _iconsOpened[day];
        _boxOpenedLastImage.transform.localScale = Vector3.one * Scale(day);
    }

    public void SetCurrent(int day, int price)
    {
        _dayLast.SetActive(false);
        _dayCurrent.SetActive(true);
        _dayNext.SetActive(false);
        _ok.gameObject.SetActive(true);
        SetColor(_currentColor);
        _dayIndicator.text = (day + 1).ToString();
        _price.text = price.ToString();
        _boxOpenedCurrentImage.sprite = _iconsOpened[day];
        _boxOpenedCurrentImage.transform.localScale = Vector3.one * Scale(day);
    }

    public void SetNext(int day, int price)
    {
        _dayLast.SetActive(false);
        _dayCurrent.SetActive(false);
        _dayNext.SetActive(true);
        _ok.gameObject.SetActive(false);
        SetColor(Color.white);
        _dayIndicator.text = (day + 1).ToString();
        _price.text = price.ToString();
        _boxOpenedNextImage.sprite = _iconsClosed[day];
        _boxOpenedNextImage.transform.localScale = Vector3.one * Scale(day);
    }

    private void SetColor(Color color)
    {
        _title.color = color;
        _dayIndicator.color = color;
        _price.color = color;
        _ok.color = color;
        _coin.color = color;
    }

    float Scale(int day)
    {
        return 1 + day * 0.2f;
    }
}
