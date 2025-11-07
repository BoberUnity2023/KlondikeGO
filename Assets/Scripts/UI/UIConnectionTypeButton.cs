using System;
using BloomLines.Assets;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UIConnectionTypeButton : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _unactiveColor;

        private Button _button;
        private ConnectionType _connectionType;
        private Action<ConnectionType> _onClick;

        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                var rect = _button.image.rectTransform;
                _button.image.color = value ? _activeColor : _unactiveColor;

                if (_isActive != value)
                {
                    DOTween.Kill(1);

                    if (value)
                    {
                        rect.localScale = Vector3.one;

                        DOTween.Sequence()
                            .Append(rect.DOScale(1.04f, 0.5f).SetEase(Ease.InOutSine))
                            .Append(rect.DOScale(1f, 0.5f).SetEase(Ease.InOutSine))
                            .SetLoops(-1)
                            .SetId(1);
                    }
                    else
                    {
                        rect.DOScale(1f, 0.5f).SetEase(Ease.InOutSine).SetId(1);
                    }
                }

                _isActive = value;
            }
        }

        public void Initialize(Action<ConnectionType> onClick)
        {
            _onClick = onClick;

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        public void SetConnectionType(ConnectionType connectionType)
        {
            _connectionType = connectionType;

            var connectionTypeData = GameAssets.GetConnectionTypeData(connectionType);
            _icon.sprite = connectionTypeData.Icon;
        }

        private void OnClick()
        {
            _onClick?.Invoke(_connectionType);
        }
    }
}