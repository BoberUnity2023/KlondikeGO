using UnityEngine;
using UnityEngine.UI;

public class LocalizeSprite : MonoBehaviour
{
    [SerializeField] private LocalizeController _localizeController;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _spriteEn;
    private Sprite _spriteRu;

    private void Awake()
    {
        _spriteRu = _image.sprite;
        _localizeController.ChangeLanguage += OnChangeLanguage;
    }

    private void OnDestroy()
    {
        _localizeController.ChangeLanguage -= OnChangeLanguage;
    }

    private void OnChangeLanguage(string lang)
    {
        if (lang == "ru")        
            _image.sprite = _spriteRu;
        else
            _image.sprite = _spriteEn;
    }
}
