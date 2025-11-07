using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    [RequireComponent(typeof(Camera))]
    public class CameraAspectRatioAdaptation : AdaptationObjectBase
    {
        private Camera _camera;

        private const float MIN_ASPECT = 9f / 21f;
        private const float MAX_ASPECT = 21f / 9f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            if (eventData.AspectRatio > MAX_ASPECT)
            {
                float scaleWidth = MAX_ASPECT / eventData.AspectRatio;
                float offsetX = (1f - scaleWidth) / 2f;
                _camera.rect = new Rect(offsetX, 0f, scaleWidth, 1f);
            }
            else if (eventData.AspectRatio < MIN_ASPECT)
            {
                float scaleHeight = eventData.AspectRatio / MIN_ASPECT;
                float offsetY = (1f - scaleHeight) / 2f;
                _camera.rect = new Rect(0f, offsetY, 1f, scaleHeight);
            }
            else
            {
                _camera.rect = new Rect(0f, 0f, 1f, 1f);
            }
        }
    }
}