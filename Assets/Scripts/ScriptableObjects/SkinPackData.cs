using System.Linq;
using UnityEngine;

namespace BloomLines.Assets
{
    [System.Serializable]
    public class PlantSkinData
    {
        [SerializeField] private PlantType _type;
        [SerializeField] private Sprite[] _plantAges;
        [SerializeField] private Sprite _cutPlant;

        public PlantType Type => _type;
        public Sprite[] PlantAges => _plantAges;
        public Sprite CutPlant => _cutPlant;
    }

    [System.Serializable]
    public class SellItemSkinData
    {
        [SerializeField] private ConnectionType _connectionType;
        [SerializeField] private PlantType _plantType;
        [SerializeField] private Sprite _icon;

        public Sprite Icon => _icon;
        public ConnectionType ConnectionType => _connectionType;
        public PlantType PlantType => _plantType;
    }

    [CreateAssetMenu(fileName = "SkinPackData", menuName = "BloomLines/SkinPackData")]
    public class SkinPackData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private PlantSkinData[] _plantSkinDatas;
        [SerializeField] private SellItemSkinData[] _sellItemSkinDatas;

        public string Id => _id;
        
        public PlantSkinData GetPlantSkin(PlantType type)
        {
            return _plantSkinDatas.FirstOrDefault(e => e.Type == type);
        }

        public SellItemSkinData GetSellItemSkin(ConnectionType connectionType, PlantType plantType)
        {
            return _sellItemSkinDatas.FirstOrDefault(e => e.ConnectionType == connectionType && e.PlantType == plantType);
        }
    }
}