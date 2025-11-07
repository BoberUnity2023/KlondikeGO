using BloomLines.Assets;
using UnityEngine;

namespace BloomLines.Audio
{
    // Скрипт от которого будут наследоваться все звуковые обьекты
    [RequireComponent(typeof(AudioSource))]
    public abstract class AudioObjectBase : MonoBehaviour
    {
        protected AudioData _data;
        protected AudioSource _source;

        public virtual void Set(AudioData data)
        {
            _source = GetComponent<AudioSource>();
            _source.volume = data.Volume;
            _source.outputAudioMixerGroup = data.Group;

            _data = data;
        }
    }
}