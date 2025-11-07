namespace BloomLines.Audio
{
    public class SoundObject : AudioObjectBase
    {
        public void PlayRandom()
        {
            _source.PlayOneShot(_data.GetClip());
        }
    }
}