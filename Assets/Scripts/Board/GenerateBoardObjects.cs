using System.Collections.Generic;
using System.Linq;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Boards
{
    // Скрипт которые контролирует что спавниться на доске
    public static class GenerateBoardObjects
    {
        // Данные сколько обьектов может быть на слое.
        // Слой 0 может быть сколько угодно обьектов (обычные растения).
        // Слой 1. Может быть только 3 обьекта на доске (кристальный цветок, камень, крот)
        private static int[] MaxCountObjectsInBoardLayers = { int.MaxValue, 3 };

        // Получаем обьекты которые должны сгенерироваться
        public static List<BoardObjectData> Generate()
        {
            var result = new List<BoardObjectData>();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null)
            {
                var balanceData = BalanceManager.Get(); // Получаем текущий игровой баланс

                TryGeneratePlants(balanceData, result); // Проверяем растения для спавна

                if (gameModeState.Type == GameModeType.Adventure) 
                {
                    TryGenerateRocks(balanceData, result); // Проверяем камни для спавна
                    TryGenerateMole(balanceData, result); // Проверяем кротов для спавна
                }
            }

            return result;
        }

        // Проверяем спавняться ли кроты
        private static void TryGenerateMole(BalanceData balanceData, List<BoardObjectData> result)
        {
            float value = Random.Range(0f, 1f);
            if (value <= balanceData.ChanceToSpawnMole) // Прокнул шанс на спавн крота
            {
                var moles = new List<MoleData>();
                foreach (var boardObject in GameAssets.BoardObjectDatas.Values) // Проходимся по всем обьектам которые могут быть на доске
                {
                    if (boardObject is MoleData) // Ищем всех кротов
                    {
                        var eventData = new GetBoardObjectCountWithLayerEvent(boardObject.ObjectsLayer);
                        EventsManager.Publish(eventData);

                        int maxCountInBoard = MaxCountObjectsInBoardLayers[boardObject.ObjectsLayer]; 

                        if (eventData.Count < maxCountInBoard) // Может ли обьект с этим слоем заспавниться, есть ли свободное место
                            moles.Add(boardObject as MoleData);
                    }
                }

                if (moles.Count > 0) // Если разные типы кротов берем рандомного
                    result.Add(moles[Random.Range(0, moles.Count)]);
            }
        }

        // Проверяем спавняться ли камни
        private static void TryGenerateRocks(BalanceData balanceData, List<BoardObjectData> result)
        {
            float value = Random.Range(0f, 1f);
            if (value <= balanceData.ChanceToSpawnRock) // Прокнул шанс на спавн камня
            {
                var rocks = new List<RockData>();
                foreach(var boardObject in GameAssets.BoardObjectDatas.Values) // Проходимся по всем обьектам которые могут быть на доске
                {
                    if(boardObject is RockData) // Ищем все камни
                    {
                        var eventData = new GetBoardObjectCountWithLayerEvent(boardObject.ObjectsLayer);
                        EventsManager.Publish(eventData);

                        int maxCountInBoard = MaxCountObjectsInBoardLayers[boardObject.ObjectsLayer];

                        if (eventData.Count < maxCountInBoard) // Может ли обьект с этим слоем заспавниться, есть ли свободное место
                            rocks.Add(boardObject as RockData);
                    }
                }

                if(rocks.Count > 0) // Если камней несколько берем рандомный
                    result.Add(rocks[Random.Range(0, rocks.Count)]);
            }
        }

        // Проверяем спавняться ли растения
        private static void TryGeneratePlants(BalanceData balanceData, List<BoardObjectData> result)
        {
            // Исходя из баланса берем количество цветов что должно заспавниться
            int generateCount = Random.Range(balanceData.GeneratePlantsCountRange.x, balanceData.GeneratePlantsCountRange.y + 1);

            for (int i = 0; i < generateCount; i++)
            {
                var plantData = GeneratePlantData(); // Берем рандомное растение
                while (true) // Если нет свободного места для этого растения то берем другое пока не найдем нужное свободное
                {
                    var eventData = new GetBoardObjectCountWithLayerEvent(plantData.ObjectsLayer);
                    EventsManager.Publish(eventData);

                    int maxCountInBoard = MaxCountObjectsInBoardLayers[plantData.ObjectsLayer];

                    if (eventData.Count < maxCountInBoard)
                        break;

                    plantData = GeneratePlantData();
                }

                result.Add(plantData);
            }
        }

        // Получаем рандомное растение
        private static PlantData GeneratePlantData()
        {
            var gameModeState = SaveManager.GameModeState;

            float totalChance = 0f;

            var plants = new HashSet<PlantData>();
            foreach(var boardObject in GameAssets.BoardObjectDatas.Values) // Берем все обьекты ТИП РАСТЕНИЕ
            {
                if(boardObject is PlantData)
                {
                    var plantData = boardObject as PlantData;
                    if (plantData.PlantType == PlantType.Flower_Crystal && gameModeState.Type == GameModeType.Classic) // в классическом режиме игры не спавним кристальный цветок
                        continue;

                    plants.Add(plantData);
                    totalChance += plantData.SpawnRarity;
                }
            }

            float currentSum = 0f;
            float randomValue = Random.Range(0f, totalChance);

            foreach (var plant in plants) // Исходя из редкости спавна берем рандомное
            {
                currentSum += plant.SpawnRarity;
                if (randomValue <= currentSum)
                    return plant;
            }

            return plants.First();
        }
    }
}