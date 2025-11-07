using BloomLines.Boards;
using UnityEngine;

namespace BloomLines.Assets
{
    public enum BoardObjectType
    {
        None,
        Plant,
        Rock,
        Mole,
    }

    public abstract class BoardObjectData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _objectsLayer;
        [SerializeField] private BoardObjectBase _objectPrefab;
        [SerializeField] private BoardObjectType _objectType;
        [SerializeField, Tooltip("Можно ли кликнуть по обьекту чтобы переместить его")] private bool _canMove;
        [SerializeField, Tooltip("Можно ли переставить обьект (считается вторым кликом по тайлу а не основным)")] private bool _canReplace;

        public string Id => _id;
        public int ObjectsLayer => _objectsLayer;
        public BoardObjectBase ObjectPrefab => _objectPrefab;
        public BoardObjectType ObjectType => _objectType;
        public bool CanMove => _canMove;
        public bool CanReplace => _canReplace;
    }
}