using System;
using BloomLines.Assets;
using BloomLines.Helpers;
using BloomLines.ObjectPool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BloomLines.Boards
{
    public enum CutPlantType
    {
        Flower,
        Crystal,
        Weed,
    }

    // Растение которое срезали
    public class CutPlant : PoolObject
    {
        [SerializeField] private PlantData _plantData;
        [SerializeField] private Transform _leafParticles;
        [SerializeField] private float _leafMainScale;
        [SerializeField] private Vector2 _mainSize;
        [SerializeField] private AnimationCurve _scaleShowCurve;
        [SerializeField] private AnimationCurve _fallCurve;
        [SerializeField] private AnimationCurve _fallScaleCurve;
        [SerializeField] private AnimationCurve _flyFirstCurve;
        [SerializeField] private AnimationCurve _flySecondCurve;

        private Image _image;
        private BoardTile _tile;

        public bool IsCollected { get; private set; }
        public PlantData PlantData => _plantData;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Update() // Обновляем размер в зависимости от размера тайлов поля
        {
            if (_tile != null)
            {
                if (_image.rectTransform.sizeDelta != _tile.RectTransform.sizeDelta)
                {
                    var scaleDiff = (Vector3)(_tile.RectTransform.rect.size / _mainSize);
                    scaleDiff.z = scaleDiff.x;

                    _leafParticles.localScale = scaleDiff * _leafMainScale;
                }

                _image.rectTransform.sizeDelta = _tile.RectTransform.sizeDelta;
            }
        }

        // Выставляем тайл и растение на котором срезали
        public void Set(BoardTile tile, Plant plant, CutPlantType plantType)
        {
            IsCollected = false; // Растение не собрано

            _tile = tile;

            _image.rectTransform.rotation = Quaternion.identity;
            _image.rectTransform.position = plant.RectTransform.position;

            _image.color = new Color(1f, 1f, 1f, 0f);

            gameObject.SetActive(true);

            var sequence = DOTween.Sequence(); // Плавное появление цветом
            sequence.Join(_image.DOFade(1f, 0.1f));

            switch (plantType)
            {
                case CutPlantType.Flower: // Если цветок
                    _image.rectTransform.localScale = Vector3.one * 0.7f;
                    sequence.Append(_image.rectTransform.DOScale(1f, 0.2f).SetEase(_scaleShowCurve)); // Плавное появление размером
                    break;
                case CutPlantType.Weed: // Если сорняк
                    _image.rectTransform.localScale = Vector3.one;

                    var fallDirection = Random.Range(-1f, 1f);
                    var position = _image.rectTransform.anchoredPosition;

                    sequence // Сорняк улетает в сторону и пропадает
                        .Append(_image.rectTransform.DOAnchorPosX(position.x - (tile.RectTransform.sizeDelta.x * fallDirection), 0.5f).SetEase(Ease.InSine))
                        .Join(_image.rectTransform.DOAnchorPosY(position.y + tile.RectTransform.sizeDelta.y, 0.5f).SetEase(_fallCurve))
                        .Join(_image.rectTransform.DOScale(0f, 0.5f).SetEase(_fallScaleCurve))
                        .Join(_image.rectTransform.DOLocalRotate(Vector3.forward * Random.Range(25f, 75f) * fallDirection, 0.5f).SetEase(Ease.InSine))
                        .AppendCallback(() => Hide(0f));
                    break;
            }
        }

        // Собираем это срезанное растение в нужную позицию
        public void FlyCollect(Vector3 position, bool zeroScaleInEnd, Action onComplete)
        {
            IsCollected = true;

            _image.rectTransform.localScale = Vector3.one;

            float value = Random.Range(0f, 1f);
            var firstCurve = Curves.Lerp(_flyFirstCurve, _flySecondCurve, value); // Рандомная кривая между двумя другими
            var secondCurve = Curves.Lerp(_flyFirstCurve, _flySecondCurve, 1f - value); // Рандомная кривая между двумя другими

            var sequence = DOTween.Sequence();

            sequence
                .AppendInterval(0.5f)
                .Append(_image.rectTransform.DOMoveX(position.x, 0.7f).SetEase(firstCurve))
                .Join(_image.rectTransform.DOMoveY(position.y, 0.7f).SetEase(secondCurve));

            if (zeroScaleInEnd)
                sequence.Join(_image.rectTransform.DOScale(0f, 0.7f).SetEase(_fallScaleCurve));
            else
                sequence.Join(_image.rectTransform.DOScale(1f, 0.7f).SetEase(_fallScaleCurve));

            sequence.AppendCallback(() =>
            {
                Hide(1f);
                onComplete?.Invoke();
            });
        }

        // Выключаем обьект через нужное время
        private void Hide(float disableDelay)
        {
            if (disableDelay <= 0f)
                gameObject.SetActive(false);
            else
            {
                _image.rectTransform.localScale = Vector3.zero;
                DOTween.Sequence().AppendInterval(disableDelay).AppendCallback(() => gameObject.SetActive(false));
            }
        }
    }
}