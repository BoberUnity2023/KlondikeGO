using BloomLines.Assets;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Boards
{
    public class Rock : BoardObjectBase
    {
        [SerializeField] private Image _img;

        public override void OnSpawn()
        {
            base.OnSpawn();

            var rockData = _data as RockData;
            _img.sprite = rockData.GetRandomSprite(); // Берем рандомную текстуру камня

            _img.rectTransform.offsetMin = new Vector2(-3f, -3f);
            _img.rectTransform.offsetMax = new Vector2(3f, 3f);

            _img.rectTransform.anchoredPosition = Vector2.up * 75f;
            _img.rectTransform.localScale = Vector3.zero;

            DOTween.Sequence() // Анимированное появление
                .Append(_img.rectTransform.DOAnchorPosY(0f, 0.6f).SetEase(Ease.OutSine))
                .Join(_img.rectTransform.DOScale(1f, 0.25f).SetEase(Ease.InSine));
        }

        // Нечего сбрасывать у камня
        public override void ResetEvolve()
        {
        }

        // Камень не еволюционирует
        public override void Evolve()
        {
            return;
        }

        // Можно ли двигать камень
        public override bool CanMove()
        {
            return _data.CanMove;
        }

        // Можно ли переставить камень
        public override bool CanReplace()
        {
            return _data.CanReplace;
        }

        // Можно ли пройти сквозь камень
        public override bool CanMoveThrough()
        {
            return false;
        }

        // Можно ли использовать конкретный инструмент на камне
        public override bool CanUseTool(ToolType type)
        {
            return type == ToolType.Pickaxe;
        }
    }
}