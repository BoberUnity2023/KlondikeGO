using System.Collections;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Adaptation
{
    public class BoardBoundsAdaptation : AdaptationObjectBase
    {
        [SerializeField] private RectTransform _board;
        [SerializeField] private RectTransform _canvasRect;
        [SerializeField] private RectTransform _top;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _left;
        [SerializeField] private RectTransform _right;

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            StartCoroutine(UpdateSize(eventData));
        }

        private IEnumerator UpdateSize(ScreenInfoEvent eventData)
        {
            yield return null;

            var boardSize = _board.rect.size;

            Vector2 boardLocalPos = _canvasRect.InverseTransformPoint(_board.position);

            _left.anchoredPosition = Vector2.up * boardLocalPos.y;
            _left.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (_canvasRect.rect.size.x - boardSize.x) / 2f + boardLocalPos.x);
            _left.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardSize.y);

            _right.anchoredPosition = Vector2.up * boardLocalPos.y;
            _right.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (_canvasRect.rect.size.x - boardSize.x) / 2f - boardLocalPos.x);
            _right.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardSize.y);

            _top.anchoredPosition = Vector2.right * boardLocalPos.x;
            _top.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (_canvasRect.rect.size.y - boardSize.y) / 2f - boardLocalPos.y);
            _top.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardSize.x);

            _bottom.anchoredPosition = Vector2.right * boardLocalPos.x;
            _bottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (_canvasRect.rect.size.y - boardSize.y) / 2f + boardLocalPos.y);
            _bottom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardSize.x);
        }
    }
}