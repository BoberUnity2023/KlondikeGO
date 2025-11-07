using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Audio;
using UnityEngine;

namespace BloomLines
{
    public static class AudioController
    {
        private static Dictionary<string, SoundObject> _sounds; // Все звуки

        // Инициализируем при загрузке сцены
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var parent = new GameObject("AudioController"); // Родитель всех звуков

            _sounds = new Dictionary<string, SoundObject>();

            InitializeSound(parent.transform);
            InitializeMusic(parent.transform);

            GameObject.DontDestroyOnLoad(parent);
        }

        // Загружаем все звуки
        private static void InitializeSound(Transform parent)
        {
            var soundDatas = Resources.LoadAll<SoundData>("Data/Audio/Sound"); // Берем все звуки

            foreach(var data in soundDatas)
            {
                var soundObject = new GameObject(data.Id).AddComponent<SoundObject>(); // Каждый звук создаем как отдельный SoundObject
                soundObject.transform.SetParent(parent);
                soundObject.Set(data);

                _sounds.Add(data.Id, soundObject);
            }
        }

        // Загружаем всю музыку
        private static void InitializeMusic(Transform parent)
        {
            var musicDatas = Resources.LoadAll<MusicData>("Data/Audio/Music"); // Берем всю музыку

            if (musicDatas == null || musicDatas.Length <= 0)
                return;

            var musicObject = new GameObject("Music").AddComponent<MusicObject>();
            musicObject.transform.SetParent(parent);
            musicObject.Set(musicDatas[0]);
        }

        // Проиграть нужный звук
        public static void Play(string id)
        {
            if (!_sounds.ContainsKey(id))
            {
                Debug.LogError("Missing audio with id: " + id);
                return;
            }

            var soundObject = _sounds[id];
            soundObject.PlayRandom();
        }
    }
}