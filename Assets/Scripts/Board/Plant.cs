using BloomLines.Assets;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Boards
{
    public class Plant : BoardObjectBase
    {
        [SerializeField] private Image[] _ageImages;
        [SerializeField] private Image _web;

        protected int _age; // Возраст расттения
        private int _webHealth; // Здоровье паутинки

        public bool HaveWeb { get; private set; }
        public PlantData PlantData => _data as PlantData;

        // Сбрасываем растение
        public override void ResetEvolve()
        {
            _canvasGroup.alpha = 1;

            _age = 0;

            foreach(var img in _ageImages)
                img.gameObject.SetActive(false);
        }

        // Еволюционируем растение
        public override void Evolve()
        {
            if (HaveWeb) // Если есть паутинка отнимает хп у неё
            {
                _webHealth--;
                if (_webHealth <= 0) // Удаляем паутинку
                    SetWeb(false);
            }

            if (_age >= PlantData.MaxAge) // Растение больше не может расти
                return;

            _age++;

            var currentAge = _ageImages[_age - 1]; // Берем иконку текущего возраста растения
            var color = currentAge.color;
            color.a = 0f;

            currentAge.color = color;
            currentAge.gameObject.SetActive(true);
            currentAge.rectTransform.localScale = Vector3.zero;

            // Плавное появляем текущий возраст
            var sequence = DOTween.Sequence()
                .Append(currentAge.rectTransform.DOScale(1f, 0.5f))
                .Join(currentAge.DOFade(1f, 0.3f));

            if (_age > 1) // Если возраст 2 и выше то прошлый возраст плавно убираем
                sequence.Join(_ageImages[_age - 2].DOFade(0f, 0.5f).SetEase(Ease.InSine)).AppendCallback(() =>
                {
                    _ageImages[_age - 2].gameObject.SetActive(false);
                });
        }

        // Можно ли двигать растение
        public override bool CanMove()
        {
            // Если само растение в данных можно двигать. Если до конца выросло. Если не разрушено. Если нет паутинки
            return _data.CanMove && _age >= PlantData.MaxAge && !_isReadyToDestroy && !HaveWeb;
        }

        // Можно ли переставить обьект
        public override bool CanReplace()
        {
            // Если его можно двигать или Растение до конца не выросло, если само растение в данных можно переставить, нет паутинки
            return CanMove() || (_age < PlantData.MaxAge && _data.CanReplace && !HaveWeb);
        }

        // Можно ли пройти сквозь
        public override bool CanMoveThrough()
        {
            // Если не выросло до конца
            return _age < 2;
        }

        // Можно ли на растении использовать конкретный инструмент
        public override bool CanUseTool(ToolType type)
        {
            var pickaxeCrystal = type == ToolType.Pickaxe && _age >= 2 && PlantData.PlantType == PlantType.Flower_Crystal; // Если инструмент кирка и растение до конца выросло и кристальный цветок 
            var secateursWeed = type == ToolType.Secateurs && PlantData.PlantType == PlantType.Weed; // Если инструмент ножницы и текущее растение сорняк
            return pickaxeCrystal || secateursWeed;
        }

        // Можно ли этим растением собрать линию
        public bool CanMakeLine()
        {
            // Если до конца выросло, если не кристальный цветок и если не разрушено
            return _age >= 2 && PlantData.PlantType != PlantType.Flower_Crystal && !_isReadyToDestroy;
        }

        private void SetAge(int age)
        {
            _age = age;

            for (int i = 0; i < _ageImages.Length; i++)
            {
                var img = _ageImages[i];
                bool currentAge = ((i + 1) == _age) || (i >= _ageImages.Length);

                if (_age >= _ageImages.Length && i >= _ageImages.Length - 1)
                    currentAge = true;

                img.gameObject.SetActive(currentAge);

                if (currentAge)
                {
                    img.rectTransform.localScale = Vector3.one;
                    img.color = Color.white;
                }
            }
        }

        public void SetWeb(bool active)
        {
            if(active != HaveWeb) // Если от прошлого состояние поставилось новое
            {
                _web.color = Color.white;
                _web.gameObject.SetActive(true);

                if (active) // Анимированно спавним
                {
                    AudioController.Play("spawn_web");

                    _web.rectTransform.localScale = Vector3.zero;
                    _web.rectTransform.DOScale(1f, 0.3f);
                    _webHealth = GameAssets.BalanceData.WebHealth;
                }
                else // Анимированно убираем
                {
                    AudioController.Play("hide_web");

                    _webHealth = 0;
                    _web.rectTransform.localScale = Vector3.one;
                    _web.DOFade(0f, 0.2f);
                }
            }

            HaveWeb = active;
        }

        private void RandomRotate()
        {
            foreach(var age in _ageImages)
                age.rectTransform.localEulerAngles = Vector3.forward * Random.Range(-15f, 15f);
        }

        public void WrongMove()
        {
            _ageImages[_age - 1].rectTransform.DOShakePosition(0.2f, 6, 20, 90, false, true, ShakeRandomnessMode.Harmonic);
        }

        #region ISaveable
        // Сохранем данные растения
        public override string GetSaveData()
        {
            if (HaveWeb) // Если есть растение то сохраняем в формате "возраст;есть ли паутинка;здоровье паутинки"
                return $"{_age};{HaveWeb};{_webHealth}";
            else // Если нет паутинки сохраняем в формате просто возраста
                return _age.ToString();
        }

        // Загружаем данные растения
        public override void LoadSaveData(string data)
        {
            var values = data.Split(';');
            
            int age = int.Parse(values[0]); // На нулевом индексе всегда возраст
            SetAge(age); 

            bool haveWeb = false;
            int webHealth = 0;
            if (values.Length > 1) // Если значение больше 1 (не только возраст) значит у растения есть паутинка
            {
                haveWeb = bool.Parse(values[1]); // есть ли паутинка
                webHealth = int.Parse(values[2]); // хп паутинки
            }

            SetWeb(haveWeb);
            if (haveWeb)
                _webHealth = webHealth;
        }
        #endregion

        private void OnEnable()
        {
            RandomRotate();
        }
    }
}