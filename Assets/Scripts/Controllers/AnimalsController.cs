using System.Collections.Generic;
using BloomLines.Animals;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Controllers
{
    // »вент дл€ добавлени€ точки спавна животного в контроллер
    public class RegisterAnimalPointEvent
    {
        public AnimalPoint Point { get; private set; }

        public RegisterAnimalPointEvent(AnimalPoint point)
        {
            Point = point;
        }
    }

    // »вент дл€ удалени€ точки спавна животного из контроллера
    public class RemoveAnimalPointEvent
    {
        public AnimalPoint Point { get; private set; }

        public RemoveAnimalPointEvent(AnimalPoint point)
        {
            Point = point;
        }
    }

    // »вент дл€ получени€ всех текущих точек животных
    public class GetAllAnimalPointsEvent
    {
        public HashSet<AnimalPoint> Points;

        public GetAllAnimalPointsEvent()
        {
            Points = new HashSet<AnimalPoint>();
        }
    }

    public class AnimalsController : MonoBehaviour
    {
        [SerializeField] private Animal[] _animals; // ¬се животные

        private HashSet<AnimalPoint> _points = new HashSet<AnimalPoint>(); // ¬се точки животных

        private void Awake()
        {
            foreach (var animal in _animals)
                animal.Initialize(this);
        }

        private void Start()
        {
            // Ѕерем все точки животных

            var eventData = new GetAllAnimalPointsEvent();
            EventsManager.Publish(eventData);

            if (eventData.Points != null && eventData.Points.Count > 0)
            {
                foreach (var point in eventData.Points)
                {
                    if(!_points.Contains(point))
                        _points.Add(point);
                }
            }
        }

        private void OnRegisterAnimalPoint(RegisterAnimalPointEvent eventData)
        {
            if (!_points.Contains(eventData.Point)) // ƒобавл€ем новую точку животного
                _points.Add(eventData.Point);
        }

        private void OnRemoveAnimalPoint(RemoveAnimalPointEvent eventData)
        {
            if(_points.Contains(eventData.Point)) // ”дал€ем точку животного
                _points.Remove(eventData.Point);
        }

        // Ѕерем рандомную свободную точку дл€ нужного животного
        public AnimalPoint GetRandomFreePoint(AnimalType type)
        {
            var availablePoints = new List<AnimalPoint>();

            foreach(var point in _points)
            {
                if (point.CanPlaceAnimal(type))
                    availablePoints.Add(point);
            }

            if (availablePoints.Count <= 0)
                return null;

            return availablePoints[Random.Range(0, availablePoints.Count)];
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<RegisterAnimalPointEvent>(OnRegisterAnimalPoint);
            EventsManager.Subscribe<RemoveAnimalPointEvent>(OnRemoveAnimalPoint);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<RegisterAnimalPointEvent>(OnRegisterAnimalPoint);
            EventsManager.Unsubscribe<RemoveAnimalPointEvent>(OnRemoveAnimalPoint);
        }
    }
}