using UnityEngine.EventSystems;

namespace BloomLines.Managers
{
    public static class InputManager
    {
        private static int _disableCount;
        private static EventSystem _system;
        
        public static void Enable() // Каждое отключенное управление должно быть включенно обратно
        {
            _disableCount--;

            if (_disableCount <= 0 && _system != null)
                _system.enabled = true;
        }

        public static void Disable() // Отключаем управление. Можно отключать из разных мест одновременно
        {
            _disableCount++;
            if (EventSystem.current != null)
            {
                _system = EventSystem.current;
                _system.enabled = false;
            }
        }
    }
}