using UnityEngine;

namespace BloomLines.Assets
{
    [CreateAssetMenu(fileName = "SellItemData", menuName = "BloomLines/SellItem")]
    public class SellItemData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _price;
        [SerializeField] private int _needFlowers;
        [SerializeField] private ConnectionType _connectionType;
        [SerializeField] private PlantType _plantType;

        public string Id => _id;
        public Sprite Icon => _icon;
        public int Price => _price;
        public int NeedFlowers => _needFlowers;
        public ConnectionType ConnectionType => _connectionType;
        public PlantType PlantType => _plantType;
    }
}