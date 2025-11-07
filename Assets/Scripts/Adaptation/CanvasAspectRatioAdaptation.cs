using BloomLines.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Adaptation
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasAspectRatioAdaptation : AdaptationObjectBase
    {
        private CanvasScaler _canvasScaler;
        private float _targetAspectRatio;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            _targetAspectRatio = _canvasScaler.referenceResolution.x / _canvasScaler.referenceResolution.y;
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            _canvasScaler.matchWidthOrHeight = eventData.AspectRatio > _targetAspectRatio ? 0f : 1f;
        }
    }
}