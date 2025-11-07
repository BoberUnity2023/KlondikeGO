using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.Assets
{
    public abstract class TaskData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private TaskType _type;
        [SerializeField] private float _rarity;
        [SerializeField] private int _stage;
        [SerializeField] private int _price;
        [SerializeField] private int[] _slotsInBenches;
        [SerializeField] private float _chanceToTimed;
        [SerializeField] private float _timeInSeconds;
        [SerializeField] private int _timeAdditionalPrice;

        public string Id => _id;
        public TaskType Type => _type;
        public float Rarity => _rarity;
        public int Stage => _stage;
        public int Price => _price;
        public int[] SlotsInBenches => _slotsInBenches;
        public float ChanceToTimed => _chanceToTimed;
        public float TimeInSeconds => _timeInSeconds;
        public int TimeAdditionalPrice => _timeAdditionalPrice;
    }
}