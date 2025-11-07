using BloomLines.Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomLines.Animals
{
    public enum AnimalType
    {
        None,
        Cat,
        Bird,
        Butterfly,
    }

    public class Animal : MonoBehaviour
    {
        [SerializeField] private AnimalType _type;
        [SerializeField] private Transform _content;

        public AnimalType Type => _type;

        private float _timeToMove;
        private Button _btn;
        private Animator _anim;
        private Transform _transform;
        private CanvasGroup _canvasGroup;
        private AnimalPoint _targetPoint;
        private AnimalsController _controller;

        private void Start()
        {
            Move();
        }

        private void Update()
        {
            if (_canvasGroup.interactable)
            {
                _timeToMove -= Time.deltaTime;
                if (_timeToMove <= 0f)
                {
                    if(Random.value < 0.3f)
                        AudioController.Play(_type.ToString().ToLowerInvariant());

                    OnClick();
                }
            }
        }

        public void Initialize(AnimalsController controller)
        {
            _transform = GetComponent<Transform>();
            _btn = _content.GetComponent<Button>();
            _canvasGroup = _content.GetComponent<CanvasGroup>();
            _anim = GetComponent<Animator>();
            _controller = controller;

            _btn.onClick.AddListener(() =>
            {
                AudioController.Play(_type.ToString().ToLowerInvariant());
                OnClick();
            });

            _canvasGroup.interactable = true;
        }

        private void OnClick()
        {
            _canvasGroup.interactable = false;
            DOTween.Sequence().Append(_canvasGroup.DOFade(0f, 0.4f)).AppendCallback(Move);
        }

        // Сменить позицию
        public void Move()
        {
            gameObject.SetActive(true);

            // Освобождаем текущую точку где сидело животное и назначаем на другую рандомную
            _targetPoint?.Clear();
            _targetPoint = _controller.GetRandomFreePoint(_type);
            _targetPoint.Set(this);

            // Берем данные от нужной точки для нашего типа животного
            var pointData = _targetPoint.GetPointData(_type);

            // Выставляем все нужные размеры и повороты
            _transform.SetParent(_targetPoint.transform);
            _transform.localPosition = Vector3.zero;
            _content.eulerAngles = pointData.GetRotation();
            _content.localScale = pointData.GetScale();

            // Плавное появление
            DOTween.Sequence().Append(_canvasGroup.DOFade(1f, 0.2f));
            _anim.SetInteger("Type", pointData.IdleType);
            _canvasGroup.interactable = true;

            // Меняем позицию через рандомное время
            _timeToMove = Random.Range(15f, 40f);
        }

        // После отключения животного включаем его обратно и двигаем
        private void OnDisable()
        {
            Invoke(nameof(Move), 0.01f);
        }
    }
}