using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Assets
{
    public abstract class CrystalRequiredToolData : ToolData
    {
        [SerializeField] private int _crystalCountRequired;

        public int CrystalCountRequired => _crystalCountRequired;
    }
}