using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UITaskGoal : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _count;
        [SerializeField] private RectTransform _countTab;

        public Sprite Icon
        {
            set
            {
                _icon.sprite = value;
            }
        }

        public int Count
        {
            set
            {
                _count.text = value.ToString();
                var sizeDelta = _countTab.sizeDelta;
                sizeDelta.x = value >= 10 ? 42f : 32f;
                _countTab.sizeDelta = sizeDelta;
            }
        }
    }
}