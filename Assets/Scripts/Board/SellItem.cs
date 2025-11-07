using System;
using BloomLines.Adaptation;
using BloomLines.Assets;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.ObjectPool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BloomLines.Boards
{
    // Предмет для продажи
    [RequireComponent(typeof(ScreenChangeNotifier))]
    public class SellItem : PoolObject
    {
        [SerializeField] private AnimationCurve _flyFirstCurve;
        [SerializeField] private AnimationCurve _flySecondCurve;
        [SerializeField] private AnimationCurve _fallScaleCurve;
        [SerializeField] private Image _icon;
        [SerializeField] private ScreenChangeNotifier _notifier; // Подписываем на события смены экрана

        private SellItemData _data;
        private BoardTile _targetTile;

        public SellItemData Data => _data;
        public RectTransform RectTransform => _icon.rectTransform;
        public bool IsInitialized { get; private set; }

        public void Initialize(Board board)
        {
            _targetTile = board.Tiles[0];
            IsInitialized = true;

            UpdateVisual();
        }

        public void Set(SellItemData data)
        {
            _data = data;

            Sprite icon = data.Icon;

            var gameState = SaveManager.GameState;
            var skinPack = GameAssets.GetSkinPackData(gameState.SkinPack); // Берем текущий скин пак
            var sellItemSkin = skinPack.GetSellItemSkin(data.ConnectionType, data.PlantType); // Берем текущее растение в нашем паке скинов
            if (sellItemSkin != null)
                icon = sellItemSkin.Icon;

            _icon.sprite = icon;

            Vector2 normalizedPivot = new Vector2(
                icon.pivot.x / icon.rect.width,
                icon.pivot.y / icon.rect.height
                );

            _icon.rectTransform.pivot = normalizedPivot;
        }

        // Обновляем размер обьекта в зависимости от размера доски
        private void UpdateVisual()
        {
            if (_targetTile != null)
                RectTransform.sizeDelta = _targetTile.RectTransform.sizeDelta;
        }

        private void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            UpdateVisual();
        }

        // Собираем предмет на нужную позицию
        public void FlyCollect(Vector3 position, float scale, float delay, bool zeroScaleInEnd, Action onComplete)
        {
            RectTransform.localScale = Vector3.one * scale;

            float value = Random.Range(0f, 1f);
            var firstCurve = Curves.Lerp(_flyFirstCurve, _flySecondCurve, value); // Рандомная кривая между двумя другими
            var secondCurve = Curves.Lerp(_flyFirstCurve, _flySecondCurve, 1f - value); // Рандомная кривая между двумя другими

            var sequence = DOTween.Sequence();

            sequence
                .AppendInterval(delay)
                .Append(RectTransform.DOMoveX(position.x, 0.7f).SetEase(firstCurve))
                .Join(RectTransform.DOMoveY(position.y, 0.7f).SetEase(secondCurve));

            if (zeroScaleInEnd)
                sequence.Join(RectTransform.DOScale(0f, 0.7f).SetEase(_fallScaleCurve));
            else
                sequence.Join(RectTransform.DOScale(scale, 0.7f).SetEase(_fallScaleCurve));

            sequence.AppendCallback(() =>
            {
                onComplete?.Invoke();
                Hide();
            });
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _notifier.OnScreenUpdate += OnScreenUpdate;

            UpdateVisual();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _notifier.OnScreenUpdate -= OnScreenUpdate;
        }
    }
}