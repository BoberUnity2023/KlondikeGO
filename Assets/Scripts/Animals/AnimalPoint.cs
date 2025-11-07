using System.Linq;
using BloomLines.Controllers;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Animals
{
    // Данные об точке для определенных животных
    [System.Serializable]
    public class AnimalPointData
    {
        [SerializeField] private AnimalType _type;
        [SerializeField] private int _idleType;
        [SerializeField] private Vector3[] _scales;
        [SerializeField] private Vector3[] _rotations;

        public AnimalType Type => _type;
        public int IdleType => _idleType;

        public Vector3 GetRotation()
        {
            if (_rotations == null || _rotations.Length <= 0)
                return Vector3.zero;

            return _rotations[Random.Range(0, _rotations.Length)];
        }

        public Vector3 GetScale()
        {
            if (_scales == null || _scales.Length <= 0)
                return Vector3.zero;

            return _scales[Random.Range(0, _scales.Length)];
        }
    }

    public class AnimalPoint : MonoBehaviour
    {
        [SerializeField] private AnimalPointData[] _datas;

        private Animal _animal;

        public void Clear()
        {
            _animal = null;
        }

        public void Set(Animal animal)
        {
            if(CanPlaceAnimal(animal.Type))
                _animal = animal;
        }

        // Получаем данные для требуемого типа животного
        public AnimalPointData GetPointData(AnimalType type)
        {
            return _datas.FirstOrDefault(e => e.Type == type);
        }

        // Проверяем свободна ли точка и может ли нужный тип животного в ней быть
        public bool CanPlaceAnimal(AnimalType type)
        {
            return _animal == null && _datas.FirstOrDefault(e => e.Type == type) != null;
        }

        private void OnGetAllAnimalPointsEvent(GetAllAnimalPointsEvent eventData)
        {
            if (!eventData.Points.Contains(this))
                eventData.Points.Add(this);
        }

        private void Awake()
        {
            EventsManager.Publish(new RegisterAnimalPointEvent(this));
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<GetAllAnimalPointsEvent>(OnGetAllAnimalPointsEvent);

            EventsManager.Publish(new RegisterAnimalPointEvent(this));
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<GetAllAnimalPointsEvent>(OnGetAllAnimalPointsEvent);

            EventsManager.Publish(new RemoveAnimalPointEvent(this));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.05f);
        }
#endif
    }
}