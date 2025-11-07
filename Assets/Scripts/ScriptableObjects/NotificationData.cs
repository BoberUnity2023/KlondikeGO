using UnityEngine;

namespace BloomLines.Assets
{
    public enum NotificationType
    {
        Default,
        TaskCompleted,
    }

    [CreateAssetMenu(fileName = "NotificationData", menuName = "BloomLines/NotificationData")]
    public class NotificationData : ScriptableObject
    {
        [SerializeField] private NotificationType _type;
        [SerializeField] private string _id;
        [SerializeField] private Sprite[] _icons;
        [SerializeField] private int _maxCount;

        public NotificationType Type => _type;
        public string Id => _id;
        public int MaxCount => _maxCount;
        public Sprite Icon => _icons[Random.Range(0, _icons.Length)];
    }
}