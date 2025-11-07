using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UIShopPanel : UIPanelBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _backgroundButton;

        protected override void Awake()
        {
            base.Awake();

            _closeButton.onClick.AddListener(() =>
            {
                Close();
                AudioController.Play("click_button");
            });

            _backgroundButton.onClick.AddListener(Close);
        }
    }
}