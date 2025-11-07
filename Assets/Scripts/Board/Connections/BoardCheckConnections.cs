using System.Collections;
using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.Tools;
using UnityEngine;

namespace BloomLines.Boards
{
    // Ивент который отсылается если линии соединенны
    public class LineConnectedEvent
    {
        private ConnectionType _connectionType; // Каким типом соединения собрали их
        private bool _isExtendedMethod; // Собрали ли их расширенным способом
        private List<HashSet<BoardTile>> _lines;

        public bool IsExtendedMethod => _isExtendedMethod;
        public ConnectionType ConnectionType => _connectionType;
        public List<HashSet<BoardTile>> Lines => _lines;

        public LineConnectedEvent(ConnectionType connectionType, bool isExtendedMethod, List<HashSet<BoardTile>> lines)
        {
            _isExtendedMethod = isExtendedMethod;
            _connectionType = connectionType;
            _lines = lines;
        }
    }

    [RequireComponent(typeof(Board))]
    public class BoardCheckConnections : MonoBehaviour // Скрипт для проверки соединений на поле
    {
        [SerializeField] private SecateursLineCutter _secateursLineCutter;

        private Dictionary<ConnectionType, ICheckConnections> _checkConnections;

        private void Awake()
        {
            _checkConnections = new Dictionary<ConnectionType, ICheckConnections>();

            // Загружаем логику всех типов соединений
            _checkConnections.Add(ConnectionType.Line4, new Line4CheckConnections());
            _checkConnections.Add(ConnectionType.Line5, new Line5CheckConnections());
            _checkConnections.Add(ConnectionType.Square, new SquareCheckConnections());
            _checkConnections.Add(ConnectionType.Triangle, new TriangleCheckConnections());
            _checkConnections.Add(ConnectionType.Plus, new PlusCheckConnections());
        }

        // Проверяем есть ли соединения
        public bool HaveAnyConnections(Board board, ConnectionType type)
        {
            var connections = _checkConnections[type];
            return connections.HaveAnyConnections(board);
        }

        // Соединяем все линии 
        public void CheckConnections(Board board, ConnectionType type)
        {
            var connections = _checkConnections[type];
            connections.CheckConnections(board);
        }

        // Если линия собранна
        private void OnLineConnected(LineConnectedEvent eventData)
        {
            Vibration.Vibrate(30);

            if (eventData.IsExtendedMethod) // Если расширенным способом собрали, показываем уведомление
            {
                EventsManager.Publish(new ShowNotificationEvent("extended_connection", string.Empty));
            }

            StartCoroutine(OnLineConnectedAnimation(eventData)); 
        }

        // Анимированно срезаем собранную линию
        private IEnumerator OnLineConnectedAnimation(LineConnectedEvent eventData)
        {
            foreach(var line in eventData.Lines)
            {
                foreach(var tile in line)
                {
                    tile.Object.IsReadyToDestroy = true;
                }
            }

            foreach(var line in eventData.Lines)
                yield return StartCoroutine(_secateursLineCutter.CutTheLine(line));
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<LineConnectedEvent>(OnLineConnected);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<LineConnectedEvent>(OnLineConnected);
        }
    }
}