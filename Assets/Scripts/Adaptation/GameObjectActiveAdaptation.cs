using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    public class GameObjectActiveAdaptation : AdaptationObjectBase
    {
        [SerializeField] private GameObject _target;

        [Header("Horizontal")]
        [SerializeField] private bool _horizontalActive;
        [Header("Vertical")]
        [SerializeField] private bool _verticalActive;

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
            _target.SetActive(screenType == ScreenType.Horizontal ? _horizontalActive : _verticalActive);
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
        }
    }
}