using System;
using System.Collections.Generic;

namespace BloomLines.Managers
{  
    public static class EventsManager
    {
        private static readonly Dictionary<Type, object> _events = new();

        public static void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var actionObj))
            {
                var action = (Action<T>)actionObj;
                action += listener;
                _events[type] = action;
            }
            else
            {
                _events[type] = listener;
            }
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var actionObj))
            {
                var action = (Action<T>)actionObj;
                action -= listener;
                if (action == null)
                    _events.Remove(type);
                else
                    _events[type] = action;
            }
        }

        public static void Publish<T>(T evt)
        {
            if (_events.TryGetValue(typeof(T), out var actionObj))
                ((Action<T>)actionObj)?.Invoke(evt);
        }
    }
}