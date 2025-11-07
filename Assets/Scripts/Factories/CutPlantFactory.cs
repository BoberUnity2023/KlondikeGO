using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class CutPlantFactory : MonoBehaviour
    {
        [SerializeField] private int _initializeCount;

        private Dictionary<PlantType, ObjectPoolController<CutPlant>> _pools;

        private void Awake()
        {
            var cutPlants = Resources.LoadAll<CutPlant>("Prefabs/CutPlants");
            _pools = new Dictionary<PlantType, ObjectPoolController<CutPlant>>();

            foreach (var cutPlant in cutPlants)
            {
                _pools.Add(cutPlant.PlantData.PlantType, new ObjectPoolController<CutPlant>(cutPlant));

                for (int i = 0; i < _initializeCount; i++)
                    _pools[cutPlant.PlantData.PlantType].GetObject();

                _pools[cutPlant.PlantData.PlantType].ResetAllObjects();
            }
        }

        public CutPlant CreateObject(PlantType plantType)
        {
            return _pools[plantType].GetObject();
        }
    }
}