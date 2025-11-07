using UnityEngine;
using UnityEngine.Audio;

namespace BloomLines.Assets
{
    public abstract class AudioData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private AudioMixerGroup _group;
        [SerializeField, Range(0, 1)] private float _volume;

        public string Id => _id;
        public AudioMixerGroup Group => _group;
        public float Volume => _volume;

        public abstract AudioClip GetClip();
    }
}