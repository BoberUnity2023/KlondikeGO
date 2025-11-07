using System.Collections;
using BloomLines.Assets;
using UnityEngine;

namespace BloomLines.Audio
{
    public class MusicObject : AudioObjectBase
    {
        public override void Set(AudioData data)
        {
            base.Set(data);

            _source.clip = data.GetClip();

            StartCoroutine(Cycle());
        }

        private IEnumerator Cycle()
        {
            var length = _source.clip.length;

            while (true)
            {
                _source.Play();
                yield return new WaitForSeconds(length + 10f);
            }
        }
    }
}