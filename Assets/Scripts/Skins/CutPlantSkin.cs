using BloomLines.Assets;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Skins
{
    public class CutPlantSkin : SkinObjectBase
    {
        [SerializeField] private PlantType _plantType;
        [SerializeField] private Image _image;

        protected override void SetSkin(SkinPackData skinPack)
        {
            var plantSkinData = skinPack.GetPlantSkin(_plantType);

            _image.sprite = plantSkinData.CutPlant;
        }
    }
}