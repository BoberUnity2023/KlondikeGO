using SimpleSolitaire.Controller;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuyWindow : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _icon2;
    [SerializeField] private GameObject _previewRectangle;
    [SerializeField] private GameObject _previewBox;
    [SerializeField] private Text _priceIndicator;
    [SerializeField] private Button _buttonBuy;

    private VisualiseElement _currentElement;    

    public void Set(VisualiseElement element)
    {
        _currentElement = element;
        _icon.sprite = element.VisualImage.sprite;
        _icon2.sprite = element.VisualImage.sprite;
        int _price = element.Price;
        _priceIndicator.text = _price.ToString();
        _buttonBuy.interactable = _gameManager.Gold >= _price;
        _buttonBuy.GetComponent<CanvasGroup>().alpha = _gameManager.Gold >= _price ? 1 : 0.5f;
        bool isRectangle = _currentElement.Type == VisualiseElementType.CardBack;
        _previewRectangle.SetActive(isRectangle);
        _previewBox.SetActive(!isRectangle);
    }

    public void OnRewardedShown()
    {
        if (_currentElement != null)
            Set(_currentElement);
    }
    
    public void PressBuy()
    {
        _gameManager.Buy(_currentElement);
    }
}
