using DG.Tweening;

namespace BloomLines.Boards
{
    public class CrystalFlower : Plant
    {
        // Еволюционируем кристальное растение
        public override void Evolve()
        {
            var oldAge = _age;
            bool ageNext = false;
            if (_age >= PlantData.MaxAge)
                ageNext = true;

            base.Evolve();

            if (oldAge == 1 && _age == 2)
            {
                AudioController.Play("spawn_crystal");
            }

            if (ageNext)
            {
                _age++;

                if(_age >= 8 && !_isReadyToDestroy) // Удаляем цветок если 8 ходов его не трогали
                {
                    AudioController.Play("hide_crystal");

                    _targetTile.RemoveObject(false);
                    DOTween.Sequence().Append(_rectTransform.DOScale(0f, 0.2f)).AppendCallback(() =>
                    {
                        gameObject.SetActive(false);
                    });

                    _isReadyToDestroy = true;
                }
            }
        }
    }
}