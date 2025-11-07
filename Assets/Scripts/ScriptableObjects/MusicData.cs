using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "BloomLines/Audio/Music")]
    public class MusicData : AudioData
    {
        [SerializeField] private AudioClip _clip;

        public override AudioClip GetClip()
        {
            return _clip;
        }
    }
}