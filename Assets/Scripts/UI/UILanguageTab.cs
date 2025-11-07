using System;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public enum LanguageCode
    {
        RU,
        EN,
        DE,
        ES,
        TR,
    }

    public class UILanguageTab : MonoBehaviour
    {
        [SerializeField] private LanguageCode _languageCode;
        [SerializeField] private Toggle _toggle;

        public Toggle Toggle => _toggle;
        public LanguageCode LanguageCode => _languageCode;

        public void Initialize(Action<LanguageCode> onSelect)
        {
            _toggle.onValueChanged.AddListener((value) =>
            {
                onSelect?.Invoke(_languageCode);
            });
        }
    }
}