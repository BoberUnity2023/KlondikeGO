using BloomLines.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Adaptation
{
    [RequireComponent(typeof(AspectRatioFitter))]
    public class AspectRatioFitterAdaptation : AdaptationObjectBase
    {
        [Header("Vertical")]
        [SerializeField] private AspectRatioFitter.AspectMode _verticalAspectMode;
        [SerializeField] private float _verticalAspectRatio;
        [Header("Horizontal")]
        [SerializeField] private AspectRatioFitter.AspectMode _horizontalAspectMode;
        [SerializeField] private float _horizontalAspectRatio;

        private AspectRatioFitter _aspectRatioFitter;

        private void Awake()
        {
            _aspectRatioFitter = GetComponent<AspectRatioFitter>();
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
            _aspectRatioFitter.aspectMode = screenType == ScreenType.Vertical ? _verticalAspectMode : _horizontalAspectMode;
            _aspectRatioFitter.aspectRatio = screenType == ScreenType.Vertical ? _verticalAspectRatio : _horizontalAspectRatio;
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
        }
    }
}