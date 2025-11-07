using UnityEngine;

namespace BloomLines.Assets
{
    public enum PlantType
    {
        Flower_Crystal,
        Flower_Orange,
        Flower_Pink,
        Flower_Purple,
        Flower_White,
        Flower_Yellow,
        Weed,
    }

    [CreateAssetMenu(fileName = "PlantData", menuName = "BloomLines/BoardObjects/Plant")]
    public class PlantData : BoardObjectData
    {
        [SerializeField] private PlantType _plantType;
        [SerializeField] private int _maxAge;
        [SerializeField] private float _spawnRarity;

        public float SpawnRarity => _spawnRarity;
        public PlantType PlantType => _plantType;
        public int MaxAge => _maxAge;
    }
}