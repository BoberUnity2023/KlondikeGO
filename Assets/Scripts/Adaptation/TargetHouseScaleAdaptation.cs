using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    public class TargetHouseScaleAdaptation : AdaptationObjectBase
    {
        [SerializeField] private RectTransform _house;
        [SerializeField] private float _baseYSize;

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            if (_house != null)
            {
                var value = (float)_house.rect.size.y / _baseYSize;
                transform.localScale = Vector3.one * value;
            }
        }
    }
}