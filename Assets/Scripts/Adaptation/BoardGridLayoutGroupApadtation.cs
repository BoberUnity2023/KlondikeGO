using System.Collections;
using BloomLines.Boards;
using BloomLines.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Adaptation
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class BoardGridLayoutGroupApadtation : AdaptationObjectBase
    {
        private RectTransform _rectTransform;
        private GridLayoutGroup _gridLayoutGroup;

        private float _totalSpacingX;
        private float _totalSpacingY;
        private float _totalPaddingX;
        private float _totalPaddingY;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();

            _totalSpacingX = _gridLayoutGroup.spacing.x * (Board.WIDTH - 1);
            _totalSpacingY = _gridLayoutGroup.spacing.y * (Board.HEIGHT - 1);
            _totalPaddingX = _gridLayoutGroup.padding.left + _gridLayoutGroup.padding.right;
            _totalPaddingY = _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom;
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            StartCoroutine(UpdateDelay());
        }

        private IEnumerator UpdateDelay()
        {
            yield return null;

            float width = _rectTransform.rect.width;
            float height = _rectTransform.rect.height;

            float cellWidth = (width - _totalSpacingX - _totalPaddingX) / 8;
            float cellHeight = (height - _totalSpacingY - _totalPaddingY) / 7;

            _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }
}