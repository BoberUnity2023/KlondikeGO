using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class CutPlantSpawner : MonoBehaviour
    {
        private CutPlantFactory _factory;

        private void Awake()
        {
            _factory = GetComponent<CutPlantFactory>();

           SpawnerManager.RegisterCutPlantSpawner(this);
        }

        public CutPlant SpawnObject(PlantType plantType)
        {
            return _factory.CreateObject(plantType);
        }
    }
}