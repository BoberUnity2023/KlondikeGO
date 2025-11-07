using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Assets
{
    public enum ToolType
    {
        None,
        Pickaxe,
        Secateurs,
        Rake,
        Pitchfork,
        Shovel,
    }

    public abstract class ToolData : ScriptableObject
    {
        [SerializeField] private ToolType _toolType;
        [SerializeField] private Sprite _activeIcon;
        [SerializeField] private Sprite _unactiveIcon;

        public ToolType ToolType => _toolType;
        public Sprite ActiveIcon => _activeIcon;
        public Sprite UnactiveIcon => _unactiveIcon;
    }
}