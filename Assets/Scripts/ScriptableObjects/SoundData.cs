using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "BloomLines/Audio/Sound")]
    public class SoundData : AudioData
    {
        [SerializeField] private AudioClip[] _clips;

        public override AudioClip GetClip()
        {
            return _clips[Random.Range(0, _clips.Length)];
        }
    }
}