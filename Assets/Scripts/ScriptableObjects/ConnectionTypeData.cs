using UnityEngine;

namespace BloomLines.Assets
{
    public enum ConnectionType
    {
        Line4,
        Line5,
        Square,
        Triangle,
        Plus,
    }

    [CreateAssetMenu(fileName = "ConnectionTypeData", menuName = "BloomLines/ConnectionTypeData")]
    public class ConnectionTypeData : ScriptableObject
    {
        [SerializeField] private ConnectionType _connectionType;
        [SerializeField] private Sprite _icon;

        public ConnectionType ConnectionType => _connectionType;
        public Sprite Icon => _icon;
    }
}