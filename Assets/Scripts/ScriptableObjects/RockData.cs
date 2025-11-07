using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "RockData", menuName = "BloomLines/BoardObjects/Rock")]
    public class RockData : BoardObjectData
    {
        [SerializeField] private Sprite[] _sprites;

        public Sprite GetRandomSprite()
        {
            return _sprites[Random.Range(0, _sprites.Length)];
        }
    }
}