using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    public class GameObjectParentAdaptation : AdaptationObjectBase
    {
        [SerializeField] private Transform _verticalParent;
        [SerializeField] private Transform _horizontalParent;

        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
            _transform.SetParent(screenType == ScreenType.Vertical ? _verticalParent : _horizontalParent);
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
        }
    }
}