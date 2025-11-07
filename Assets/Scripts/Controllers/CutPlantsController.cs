using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Managers;
using BloomLines.Tools;
using UnityEngine;

#if CONSOLE
using QFSW.QC;
#endif

namespace BloomLines.Controllers
{
    // Ивент когда ножницы срезают цветок
    public class CutPlantSpawnedEvent
    {
        public CutPlant CutPlant { get; private set; }

        public CutPlantSpawnedEvent(CutPlant cutPlant)
        {
            CutPlant = cutPlant;
        }
    }

    // Ивент для проверки можно ли собрать кристальный цветок
    public class CheckFreeCrystalSlotEvent
    {
        public bool HaveFreeSlot;
    }

    public class CutPlantsController : MonoBehaviour
    {
        // Срезаем цветок
        private void OnCutPlant(CutPlantEvent eventData)
        {
            var plantType = eventData.Plant.PlantData.PlantType;
            if (plantType == PlantType.Flower_Crystal) // Если собрали кристальный цветок
            {
                var checkFreeCrystalSlotEvent = new CheckFreeCrystalSlotEvent(); // Проверяем можно ли его собрать, если нет то не спавним его
                EventsManager.Publish(checkFreeCrystalSlotEvent);

                if (!checkFreeCrystalSlotEvent.HaveFreeSlot)
                    return;
            }

            // Спавним срезанное растение
            var cutPlant = SpawnerManager.CutPlantSpawner.SpawnObject(eventData.Plant.PlantData.PlantType);
            var cutPlantType = plantType == PlantType.Weed ? CutPlantType.Weed : (plantType == PlantType.Flower_Crystal ? CutPlantType.Crystal : CutPlantType.Flower);

            cutPlant.transform.SetParent(transform, false);
            cutPlant.Set(eventData.Tile, eventData.Plant, cutPlantType);

            EventsManager.Publish(new CutPlantSpawnedEvent(cutPlant));
        }

#if CONSOLE
        [Command("add_crystal_flowers")]
#endif
        public void AddCrystalFlowers(int count)
        {
            count = Mathf.Clamp(count, 0, 50);

            var board = FindAnyObjectByType<Board>();
            var tile = board.GetTile(3, 3);

            for (int i = 0; i < count; i++)
            {
                var plant = SpawnerManager.BoardObjectsSpawner.SpawnObject("flower_crystal") as Plant;

                plant.RectTransform.SetParent(tile.RectTransform, false);
                plant.RectTransform.localScale = Vector3.zero;
                plant.RectTransform.localPosition = Vector3.zero;

                EventsManager.Publish(new CutPlantEvent(tile, plant));
            }
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<CutPlantEvent>(OnCutPlant);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<CutPlantEvent>(OnCutPlant);
        }
    }
}