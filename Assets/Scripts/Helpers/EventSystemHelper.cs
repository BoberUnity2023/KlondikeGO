using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomLines.Helpers
{
    public class EventSystemHelper : MonoBehaviour
    {
        private EventSystem _eventSystem;

        private GameObject _lastGameObject;

        private void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Update()
        {
            if(_lastGameObject != _eventSystem.currentSelectedGameObject)
            {
                _lastGameObject = _eventSystem.currentSelectedGameObject;

                if(_lastGameObject != null && _lastGameObject.TryGetComponent<Button>(out Button btn)) // Снимаем выделение с активной кнопки чтобы она не была выделена при клике
                {
                    _lastGameObject = null;
                    _eventSystem.SetSelectedGameObject(null);
                }
            }
        }
    }
}