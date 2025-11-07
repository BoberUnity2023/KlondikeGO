using BloomLines.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Skins
{
    public class PlantSkin : SkinObjectBase
    {
        [SerializeField] private PlantType _plantType;
        [SerializeField] private Image[] _ages;

        protected override void SetSkin(SkinPackData skinPack)
        {
            var plantSkinData = skinPack.GetPlantSkin(_plantType);

            for(int i = 0; i < _ages.Length; i++)
                _ages[i].sprite = plantSkinData.PlantAges[i];
        }
    }
}