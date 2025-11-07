using System;
using BloomLines.Managers;
using BloomLines.Saving.Convertors;
using UnityEngine;

namespace BloomLines.Saving.Adapters
{
    // Базовый скрипт адаптера сохранений от которого должны наследоваться все другие типы
    public abstract class BaseAdapter : ISaveAdapter
    {
        public abstract void Initialize();
        public abstract void Sync();

        public abstract bool HasSave<T>() where T : SaveState;

        public T Load<T>() where T : SaveState
        {
            string json = LoadJson<T>();

            T result = null;
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonUtility.FromJson<T>(json);
                json = SaveConverter.Convert<T>(json, result.SaveVersion, Application.version);
                result = JsonUtility.FromJson<T>(json);
            }

            if (result == null)
                result = SaveManager.GetDefaultState<T>();

            Debug.Log($"Load '{typeof(T).Name}' - {GetType().Name}");

            return result;
        }

        public void Save<T>(T state) where T : SaveState
        {
            state.SaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var json = JsonUtility.ToJson(state);

            SaveJson<T>(json);

            Debug.Log($"Save '{typeof(T).Name}' - {GetType().Name}");
        }

        protected abstract string LoadJson<T>() where T : SaveState;
        protected abstract void SaveJson<T>(string json) where T : SaveState;
    }
}