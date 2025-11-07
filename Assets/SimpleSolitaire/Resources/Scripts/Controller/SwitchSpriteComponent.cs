using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire
{
    public class SwitchSpriteComponent : MonoBehaviour
    {
        [SerializeField] private Sprite _spriteTrue;
        [SerializeField] private Sprite _spriteFalse;
        [SerializeField] private GameObject _offIcon;
        [Space(5f)]
        [SerializeField]
        private Image _switchImage;

        public void UpdateSwitchImg(bool isTrue)
        {
            Sprite sprite = _spriteTrue;
            if (!isTrue)
            {
                sprite = _spriteFalse;
            }
            _switchImage.overrideSprite = sprite;
            _offIcon.SetActive(!isTrue);
        }
    }
}