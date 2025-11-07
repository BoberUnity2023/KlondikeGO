using BloomLines.Assets;
using DG.Tweening;
using UnityEngine;

namespace BloomLines.Boards
{
    public class Mole : BoardObjectBase
    {
        private int _age; // Возраст крота
        private Animator _anim;

        protected override void Awake()
        {
            base.Awake();

            _anim = GetComponent<Animator>();
        }

        // Анимированно кушаем растение где заспавнился крот
        public void SetEatPlant(Plant plant)
        {
            DOTween.Sequence().Append(plant.RectTransform.DOScale(0f, 0.2f)).AppendCallback(() =>
            {
                plant.gameObject.SetActive(false);
            });
        }

        // Вызывается из ивента анимации
        public void DisableMole()
        {
            gameObject.SetActive(false);
        }
        
        // Сбрасываем возраст крота
        public override void ResetEvolve()
        {
            _age = 0;
        }

        // Еволюционируем крота
        public override void Evolve()
        {
            _age++;

            if (_age >= 2 && !_isReadyToDestroy) // На второй ход удаляем его
            {
                AudioController.Play("hide_mole");

                _targetTile.RemoveObject(false);
                _anim.SetTrigger("Hide");

                _isReadyToDestroy = true;
            }
        }

        // Можно ли двигать
        public override bool CanMove()
        {
            return false;
        }

        // Можно ли пройти сквозь него
        public override bool CanMoveThrough()
        {
            return false;
        }

        // Можно ли переставить
        public override bool CanReplace()
        {
            return false;
        }

        // Можно ли на нем использовать конкретный инструмент
        public override bool CanUseTool(ToolType type)
        {
            return false;
        }

        #region ISaveable
        // Сохранем возраст крота
        public override string GetSaveData()
        {
            return _age.ToString();
        }

        // Загружаем возраст крота
        public override void LoadSaveData(string data)
        {
            int age = int.Parse(data);
        }
        #endregion
    }
}