using System.Collections;
using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.Tools;
using BloomLines.UI;
using UnityEngine;

namespace BloomLines.Controllers
{
    public class SellItemsEvent { } // Ивент при продаже товаров с лавочек

    public class ReloadBenchesEvent { } // Ивент для перезагрузки всех лавочек

    public class CollectItemToBenchEvent { } // Ивент при сборе предмета на лавочку

    public class SellAllItemsFromBenchesEvent { } // Ивент ДЛЯ ВЫЗОВА продажи всех товаров

    public class BenchesController : MonoBehaviour
    {
        [SerializeField] private RectTransform _houseSellPoint;
        [SerializeField] private RectTransform _citySellPoint;
        [SerializeField] private ParticleSystem _collectToHouseVFX;
        [SerializeField] private Bench[] _benches; // Все лавочки

        private SecateursLineCutter _lineCutter;
        private Board _board;
        private int _flyCount; // Количество летающих товаров в данный момент

        private void Awake()
        {
            _board = FindAnyObjectByType<Board>();
            _lineCutter = FindAnyObjectByType<SecateursLineCutter>(FindObjectsInactive.Include);

            foreach (var bench in _benches)
                bench.Initialize(_board);
        }

        private void OnChangeConnectionType(ChangeConnectionTypeEvent eventData)
        {
            UpdateBenchesCapacity();
        }

        // Обновляем вместимость лавочек
        private void UpdateBenchesCapacity()
        {
            var gameModeState = SaveManager.GameModeState;
            var balanceData = GameAssets.BalanceData;
            int totalItemsCount = balanceData.GetTotalItemsCountInBenches(gameModeState.ConnectionType); // Получаем количество предметов на всех лавочках
            int itemsPerBench = totalItemsCount / _benches.Length; // количество предметов на одной лавочке
            foreach (var bench in _benches)
                bench.SetMaxItemsCount(itemsPerBench);
        }

        // Загружаем лавочки
        private void LoadBenches()
        {
            foreach (var bench in _benches) // Очищаем все лавочки
                bench.Clear();

            var gameModeState = SaveManager.GameModeState;
            if (gameModeState == null || !GameModeController.IsPlaying) // Если игра не запущена или её нет
                return;

            UpdateBenchesCapacity();

            var balanceData = GameAssets.BalanceData;
            int totalItemsCount = balanceData.GetTotalItemsCountInBenches(gameModeState.ConnectionType); // Количество предметов на всех лавочках
            int itemsPerBench = totalItemsCount / _benches.Length; // Количество предметов на одной лавочке

            int count = 0;
            int index = 0;
            for (int i = 0; i < gameModeState.BenchItems.Count; i++) // Проходимся по всем предметам что есть
            {
                var itemData = GameAssets.GetSellItemData(gameModeState.BenchItems[i]); // Берем данные этого предмета
                _benches[index].Add(itemData); // Добавляем предмет на лавочку
                count++;

                if(count >= itemsPerBench)
                {
                    count = 0;
                    index++;

                    if (index >= _benches.Length)
                        break;
                }
            }
            
            if(GetFreeBench() == null) // Если лавочки заполнены
            {
                StartCoroutine(SellAllItems());
            }
        }

        // Получить свободную лавочку
        private Bench GetFreeBench()
        {
            foreach(var bench in _benches)
            {
                if (bench.HaveFreeSpace)
                    return bench;
            }

            return null;
        }

        // Когда предмет продался в городе
        private void OnSellItem(SellItemData itemData)
        {
            Vibration.Vibrate(50);

            EventsManager.Publish(new AddedScoreEvent(itemData.Price));
        }

        // Конвертируем цветочки из домика
        private void TryConvertFlowersFromHouse()
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.ConnectionType == ConnectionType.Line4)
                return;

            Dictionary<string, int> flowers = new Dictionary<string, int>();
            foreach (var item in gameModeState.FlowersInHouse)
            {
                if (!flowers.ContainsKey(item))
                    flowers.Add(item, 0);

                flowers[item]++;
            }

            foreach(var flower in flowers)
            {
                var plantData = GameAssets.GetBoardObjectData(flower.Key) as PlantData;
                if (plantData != null)
                {
                    var sellItemData = GameAssets.GetSellItemData(gameModeState.ConnectionType, plantData.PlantType);
                    int count = flower.Value / sellItemData.NeedFlowers;

                    for(int i = 0; i < count; i++)
                    {
                        var bench = GetFreeBench();
                        if (bench != null)
                        {
                            _flyCount++;

                            var sellItem = SpawnerManager.SellItemSpawner.SpawnObject();
                            sellItem.RectTransform.SetParent(bench.transform);

                            if (!sellItem.IsInitialized)
                                sellItem.Initialize(_board);

                            var scale = gameModeState.ConnectionType == ConnectionType.Line4 ? 1f : 1.3f;

                            sellItem.Set(sellItemData);
                            sellItem.RectTransform.localScale = Vector3.one * scale;
                            sellItem.RectTransform.position = _houseSellPoint.position;

                            for (int b = 0; b < sellItemData.NeedFlowers; b++)
                                gameModeState.FlowersInHouse.Remove(flower.Key);

                            var position = bench.GetItemPosition(bench.ItemsCount);
                            var sellItemInBench = bench.Add(sellItemData);
                            sellItemInBench.RectTransform.localScale = Vector3.zero;

                            gameModeState.BenchItems.Add(sellItemData.Id);
                            
                            AudioController.Play("collect_product_from_house");

                            sellItem.FlyCollect(position, scale, 0f, false, () =>
                            {
                                sellItemInBench.RectTransform.localScale = Vector3.one * scale;
                                _flyCount--;

                                if (GetFreeBench() == null && _flyCount <= 0)
                                {
                                    StartCoroutine(SellAllItems());
                                }

                                EventsManager.Publish(new CollectItemToBenchEvent());
                            });
                        }
                    }
                }
            }
        }

        // Продать все товары
        private IEnumerator SellAllItems()
        {
            InputManager.Disable(); // Блокируем управление

            while (_lineCutter.IsPlaying) // Ждем завершения анимации ножниц
                yield return null;

            if(TutorialController.IsCompleted(TutorialIds.FIRST_GAME)) // Если обучение пройдено, показываем уведомление о продаже
                EventsManager.Publish(new ShowNotificationEvent("end_season", string.Empty));

            var gameModeState = SaveManager.GameModeState;

            var allItems = new List<SellItem>();
            foreach (var bench in _benches) // Берем все предметы из лавочек
                allItems.AddRange(bench.Items);

            var balanceData = GameAssets.BalanceData;
            int totalItemsCount = balanceData.GetTotalItemsCountInBenches(gameModeState.ConnectionType);
            var wait = new WaitForSeconds(1.3f / totalItemsCount);

            while (allItems.Count > 0)
            {
                var index = Random.Range(0, allItems.Count);
                var item = allItems[index];

                // Рандомный предмет отправляем лететь в город и продаем его
                item.FlyCollect(_citySellPoint.position, item.RectTransform.localScale.x, 0f, true, () => OnSellItem(item.Data));

                allItems.RemoveAt(index);

                yield return wait;
            }

            if (gameModeState.FlowersInHouse.Count > 0) // Если в домике остались цветы продаем их сразу
            {
                int price = gameModeState.FlowersInHouse.Count;
                EventsManager.Publish(new AddedScoreEvent(price));

                gameModeState.FlowersInHouse.Clear();
            }

            yield return new WaitForSeconds(0.6f);

            foreach (var b in _benches) // Очищаем все лавочки
                b.Clear();

            gameModeState.BenchItems.Clear();
            gameModeState.MovesCountAfterSell = 0;
            gameModeState.TotalSellCount++;

            var gameState = SaveManager.GameState;
            if (gameModeState.TotalSellCount >= 3 && !gameState.IsRateGame) // Если за всё время сделал три продажи и ниразу не оценил игру
            {
                gameState.IsRateGame = true;

                bool showPanel = false;

#if Yandex
                showPanel = YG.YG2.reviewCanShow;
#endif

#if RuStore
                showPanel = true;
#endif

                // Показываем окно оценки
                if (showPanel)
                    EventsManager.Publish(new OpenPanelEvent(UIPanelType.RateGame));
            }

            EventsManager.Publish(new SellItemsEvent());
            InputManager.Enable(); // Возвращаем управление
        }

        // Когда срезанный цветок заспавнился
        private void OnCutPlantSpawned(CutPlantSpawnedEvent eventData)
        {
            // Лавочки не могут подобрать кристальный цветок и сорняк
            if (eventData.CutPlant.PlantData.PlantType == PlantType.Flower_Crystal || eventData.CutPlant.PlantData.PlantType == PlantType.Weed)
                return;

            var cutPlant = eventData.CutPlant;
            var bench = GetFreeBench();

            if (bench == null)
            {
                // Если нет свободной лавочки то цветочек улетает сразу в город и продается
                cutPlant.FlyCollect(_citySellPoint.position, true, () =>
                {
                    var sellItemData = GameAssets.GetSellItemData(ConnectionType.Line4, cutPlant.PlantData.PlantType);
                    OnSellItem(sellItemData);
                });

                return;
            }

            var gameModeState = SaveManager.GameModeState;
            var sellItemData = GameAssets.GetSellItemData(gameModeState.ConnectionType, cutPlant.PlantData.PlantType);

            _flyCount++;

            if (gameModeState.ConnectionType == ConnectionType.Line4) // Если тип соединения 4
            {
                var position = bench.GetItemPosition(bench.ItemsCount);
                var sellItem = bench.Add(sellItemData);
                sellItem.RectTransform.localScale = Vector3.zero;

                // Цветочек летит на свободную позицию на лавочке
                cutPlant.FlyCollect(position, false, () =>
                {
                    sellItem.RectTransform.localScale = Vector3.one;

                    _flyCount--;

                    // Если прилетел и лавочки заполнились, продаем содержимое
                    if (GetFreeBench() == null && _flyCount <= 0)
                    {
                        StartCoroutine(SellAllItems());
                    }

                    EventsManager.Publish(new CollectItemToBenchEvent());
                });

                gameModeState.BenchItems.Add(sellItemData.Id);
            }
            else // Любой другой тип соединения
            {
                // Отправляем лететь все цветочки в домик
                cutPlant.FlyCollect(_houseSellPoint.position, true, () =>
                {
                    _collectToHouseVFX.Play();

                    _flyCount--;
                    gameModeState.FlowersInHouse.Add(cutPlant.PlantData.Id);
                    TryConvertFlowersFromHouse(); // Пытаемся конвертировать цветочки из домика
                });
            }
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            LoadBenches();
        }

        private void OnSellAllItemsFromBenches(SellAllItemsFromBenchesEvent eventData)
        {
            StartCoroutine(SellAllItems());
        }

        private void OnReloadBenches(ReloadBenchesEvent eventData)
        {
            LoadBenches();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<ReloadBenchesEvent>(OnReloadBenches);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Subscribe<CutPlantSpawnedEvent>(OnCutPlantSpawned);
            EventsManager.Subscribe<ChangeConnectionTypeEvent>(OnChangeConnectionType);
            EventsManager.Subscribe<SellAllItemsFromBenchesEvent>(OnSellAllItemsFromBenches);

            LoadBenches();
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<ReloadBenchesEvent>(OnReloadBenches);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Unsubscribe<CutPlantSpawnedEvent>(OnCutPlantSpawned);
            EventsManager.Unsubscribe<ChangeConnectionTypeEvent>(OnChangeConnectionType);
            EventsManager.Unsubscribe<SellAllItemsFromBenchesEvent>(OnSellAllItemsFromBenches);
        }
    }
}