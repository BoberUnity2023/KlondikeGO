using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Assets
{
    public abstract class EvolveRequiredToolData : ToolData
    {
        [SerializeField] private int _evolveCountRequired;

        public int EvolveCountRequired => _evolveCountRequired;
    }
}