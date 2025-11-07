using System.Collections;
using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Cursor;
using BloomLines.Helpers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.UI;
using DG.Tweening;
using Lines.Helpers;
using Unity.VisualScripting;
using UnityEngine;

namespace BloomLines.Boards
{
    public class CheckGameCompleteEvent { } // Ивент для проверки завершена ли игра

    public class BoardEvolveEvent { } // Ивент который срабатывает при еволюции всей доски

    public class GetBoardObjectCountWithLayerEvent // Ивент для получения количества обьектов на доске с нужным слоем
    {
        public int Layer { get; private set; }
        public int Count { get; private set; }

        public void Set(int count)
        { 
            Count = count;
        }

        public GetBoardObjectCountWithLayerEvent(int layer)
        {
            Layer = layer;
        }
    }

    public class Board : MonoBehaviour, IBalanceModifier
    {
        [SerializeField] private BoardTile[] _tiles;

        public BoardTile[] Tiles => _tiles;

        public const int WIDTH = 8;
        public const int HEIGHT = 7; 

        private bool _isEnded; // завершена ли игра
        private float _autoSaveTime;
        private BoardCheckConnections _checkConnections;

        private void Awake()
        {
            _checkConnections = GetComponent<BoardCheckConnections>();

            for(int i = 0; i < _tiles.Length; i++)
            {
                int x = i % WIDTH;
                int y = i / WIDTH;

                _tiles[i].Initialize(i, x, y);
            }
        }

        private void Update()
        {
            var gameModeState = SaveManager.GameModeState;
            if(!_isEnded && gameModeState != null)
            {
                _autoSaveTime += Time.deltaTime;
                if(_autoSaveTime >= 30f)
                {
                    _autoSaveTime = 0f;

                    SaveManager.Save(SaveType.GameMode);
                    SaveManager.Sync();
                }
            }
        }

        // Еволюционируем всю доску
        public void Evolve(bool withCallback, bool spawnNewObjects)
        {
            foreach (var tile in _tiles) // Еволюционируем все обьекты на доске
                tile.Evolve();

            if(spawnNewObjects)
                SpawnNewObjects();

            if (withCallback) 
                EventsManager.Publish(new BoardEvolveEvent());

            if (!CanMakeAnyMove()) // Если нет ходов то еволюционируем доску еще раз (потому что ростки двигать нельзя)
                Evolve(withCallback, spawnNewObjects);
        }

        // Спавним новые обьекты на доске
        public void SpawnNewObjects()
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState.Type == GameModeType.Adventure) // В режиме приключения пробуем спавнить паутинку
                TrySpawnWeb();

            var newBoardObjects = GenerateBoardObjects.Generate(); // Получаем обьекты которые будем генерировать

            foreach (var newObject in newBoardObjects)
            {
                switch (newObject.ObjectType)
                {
                    case BoardObjectType.Mole: // Если генерируемый предмет это крот
                        var plantTile = GetRandomTileWithPlant();
                        if (plantTile != null)
                        {
                            AudioController.Play("spawn_mole");
                            Vibration.Vibrate(30);

                            var plant = plantTile.Object as Plant; // Удаляем данное растение с тайла
                            plantTile.RemoveObject(false);

                            var moleObject = SpawnNewObjectInTile(newObject.Id, plantTile) as Mole;
                            moleObject.OnSpawn();
                            moleObject.SetEatPlant(plant); // указываем какое растение сьел крот
                        }
                        break;
                    case BoardObjectType.Plant:
                    case BoardObjectType.Rock: // Если генерируемый предмет это растение или камень
                        var tile = GetRandomEmptyTile(); 
                        if (tile != null)
                        {
                            if(newObject.ObjectType == BoardObjectType.Rock)
                                AudioController.Play("spawn_rock");

                            var boardObject = SpawnNewObjectInTile(newObject.Id, tile);
                            boardObject.OnSpawn();
                        }
                        break;
                }
            }
        }

        private void TrySpawnWeb()
        {
            var balanceData = BalanceManager.Get(); // Берем данные о текущем балансе игры

            float value = Random.Range(0f, 1f);
            if (value <= balanceData.ChanceToSpawnWeb) // Если прокнул шанс на спавн паутинки
            {
                var plants = new List<Plant>();

                foreach(var tile in _tiles) // Берем рандомное растение у которого нет паутинки
                {
                    if(tile.HaveObject() && tile.Object is Plant)
                    {
                        var plant = tile.Object as Plant;
                        if (!plant.HaveWeb)
                            plants.Add(plant);
                    }
                }

                if(plants.Count > 0)
                {
                    plants[Random.Range(0, plants.Count - 1)].SetWeb(true);
                }
            }
        }

        // Проверка есть ли возможность сделать любой ход
        public bool CanMakeAnyMove()
        {
            foreach(var tile in _tiles)
            {
                if (tile.HaveObject() && tile.Object.CanMove())
                    return true;
            }

            return false;
        }

        private void LoadBoardState(GameModeState state)
        {
            var boardState = state.BoardState;

            ClearBoard();

            for(int i = 0; i < boardState.Tiles.Length; i++)
            {
                var tile = boardState.Tiles[i];

                if (tile.HaveObject())
                {
                    var boardObject = SpawnNewObjectInTile(tile.ObjectState.Id, _tiles[i]);
                    boardObject.LoadSaveData(tile.ObjectState.Data);
                }
            }
        }

        public void ClearBoard()
        {
            foreach(var tile in _tiles)
                tile.RemoveObject(true);
        }

        // Спавним нужный обьект на нужном тайле
        public BoardObjectBase SpawnNewObjectInTile(string id, BoardTile tile)
        {
            var boardObject = SpawnerManager.BoardObjectsSpawner.SpawnObject(id);

            boardObject.ResetEvolve();
            boardObject.Evolve();

            tile.PlaceObject(boardObject);

            return boardObject;
        }

        private void OnCheckGameComplete(CheckGameCompleteEvent eventData)
        {
            if (_isEnded)
                return;

            var gameModeState = SaveManager.GameModeState;
            if (_checkConnections.HaveAnyConnections(this, gameModeState.ConnectionType)) // Проверка есть ли соединенные линии
            {
                gameModeState.MovesWithoutMoves = 0;
                return;
            }

            bool haveFreeTile = false;
            foreach (var tile in _tiles) // Проверяем есть ли пустой тайл
            {
                if (!tile.HaveObject())
                {
                    haveFreeTile = true;
                    break;
                }
            }

            if (!haveFreeTile) // Если нет пустого тайла
            {
                gameModeState.MovesWithoutMoves++;
                if (gameModeState.MovesWithoutMoves >= 2) // Если игрок сделал два раза сделал ход в момент когда поле было заполненно
                {
                    _isEnded = true;
                    EventsManager.Publish(new EndGameModeEvent());
                }
            }
        }

        public bool CanReplace(int from, int to)
        {
            var fromTile = _tiles[from];
            var toTile = _tiles[to];

            if (!fromTile.HaveObject() || !fromTile.Object.CanMove()) // Нет в первом тайле обьекта или его нельзя двигать
                return false;

            if (toTile.HaveObject() && !toTile.Object.CanReplace()) // Нельзя поставить/переставить на второй тайл
                return false;

            var path = AStarPathFinding.FindPath(_tiles, from, to, WIDTH, HEIGHT);
            if (path == null) // нет пути как переставить
                return false;

            return true;
        }

        // Меняем местами обьекты на двух тайлах
        public IEnumerator ReplaceObjects(int from, int to)
        {
            if (!CanReplace(from, to))
                yield break;

            var fromTile = _tiles[from];
            var toTile = _tiles[to];

            var startObject = fromTile.Object;
            var endObject = toTile.Object;

            startObject.CanvasGroup.DOFade(0f, 0.25f);
            if (endObject != null)
                endObject.CanvasGroup.DOFade(0f, 0.25f);

            yield return new WaitForSeconds(0.3f);

            fromTile.RemoveObject(false);
            if (toTile.HaveObject())
                toTile.RemoveObject(false);

            if (endObject != null)
                fromTile.PlaceObject(endObject);
            toTile.PlaceObject(startObject);

            startObject.CanvasGroup.DOFade(1f, 0.15f);
            if (endObject != null)
                endObject.CanvasGroup.DOFade(1f, 0.15f);

            var gameModeState = SaveManager.GameModeState;
            if (!_checkConnections.HaveAnyConnections(this, gameModeState.ConnectionType)) // Если нет линий которые соединили, то еволюционируем всю доску
                Evolve(true, true);

            OnCheckGameComplete(null);

            _checkConnections.CheckConnections(this, gameModeState.ConnectionType); // Собираем соединенные линии
        }

        #region GetTile
        private BoardTile GetRandomTileWithPlant()
        {
            HashSet<int> tileIndexes = new HashSet<int>(_tiles.Length);
            foreach (var tile in _tiles)
                tileIndexes.Add(tile.Index);

            for (int i = 0; i < _tiles.Length; i++)
            {
                var index = Random.Range(0, tileIndexes.Count);
                var tile = _tiles[index];
                if (tile.HaveObject() && tile.Object is Plant)
                    return tile;

                tileIndexes.Remove(index);
            }

            return null;
        }

        private BoardTile GetRandomEmptyTile()
        {
            HashSet<int> tileIndexes = new HashSet<int>(_tiles.Length);
            foreach (var tile in _tiles)
                tileIndexes.Add(tile.Index);

            for (int i = 0; i < _tiles.Length; i++)
            {
                var index = Random.Range(0, tileIndexes.Count);
                var tile = _tiles[index];
                if (!tile.HaveObject())
                    return tile;

                tileIndexes.Remove(index);
            }

            return null;
        }

        public BoardTile GetTile(int x, int y)
        {
            int index = (y * WIDTH) + x;
            return _tiles[index];
        }

        private bool TryGetTile(int x, int y, out BoardTile tile)
        {
            if (x < 0 || y < 0 || x >= WIDTH || y >= HEIGHT)
            {
                tile = null;
                return false;
            }

            tile = GetTile(x, y);
            return true;
        }

        // Берем тайлы в нужном радиусе (только границы без центра)
        public HashSet<BoardTile> GetTilesInRadius(int startX, int startY, int radius)
        {
            var result = new HashSet<BoardTile>();

            if (radius == 0)
            {
                result.Add(GetTile(startX, startY));
            }
            else
            {
                BoardTile tile = null;
                for (int i = -radius; i <= radius; i++)
                {
                    if (TryGetTile(startX + i, startY - radius, out tile))
                        result.Add(tile);

                    if (TryGetTile(startX + i, startY + radius, out tile))
                        result.Add(tile);

                    if (TryGetTile(startX - radius, startY + i, out tile))
                        result.Add(tile);

                    if (TryGetTile(startX + radius, startY + i, out tile))
                        result.Add(tile);
                }
            }

            return result;
        }

        // Берем все тайлы в радиусе (включая центральные)
        public HashSet<BoardTile> GetAllTilesInRadius(int startX, int startY, int radius)
        {
            var result = new HashSet<BoardTile>();
            for (int i = 0; i <= radius; i++)
                result.AddRange(GetTilesInRadius(startX, startY, i));

            return result;
        }
        #endregion

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            LoadBoardState(eventData.State);

            _isEnded = false;

            if (!eventData.IsContinue) // Если новая игра
            {
                ClearBoard();

                Evolve(false, true);
            }

            OnCheckGameComplete(null);
        }
        
        // Сохраняем доску
        private void OnSave(SaveEvent eventData)
        {
            if (eventData.Type != SaveType.GameMode || eventData.Phase != SavePhase.Prepare)
                return;

            foreach (var tile in _tiles)
                tile.Save();
        }

        // Когда отпустили предмет с рук
        private void OnReleaseTool(ReleaseToolEvent eventData)
        {
            foreach (var tile in _tiles) // Убираем выделения на всех тайлах
            {
                tile.IsHighlight = false;
                tile.IsDotHighlight = false;
            }
        }

        // Записываем сколько обьектов на нужном слое
        private void OnGetBoardObjectCountWithLayer(GetBoardObjectCountWithLayerEvent eventData)
        {
            int result = 0;
            foreach (var tile in Tiles)
            {
                if (tile.HaveObject())
                {
                    if (tile.Object.Data.ObjectsLayer == eventData.Layer)
                        result++;
                }
            }

            eventData.Set(result);
        }

        // Когда возродились за рекламу
        private void OnRevive(ReviveEvent eventData)
        {
            _isEnded = false;
            var tiles = new List<BoardTile>(_tiles);

            float clearPercent = 0.3f; // очищаем 30% от всей доски
            for(int i = 0; i < _tiles.Length * clearPercent; i++)
            {
                var tile = tiles[Random.Range(0, tiles.Count)];

                tile.RemoveObject(true);
                tiles.Remove(tile);
            }

            SaveManager.Save(SaveType.GameMode);
            SaveManager.Sync();
        }

        #region IBalanceModifier
        public int Priority => 10;

        public void Apply(BalanceData data)
        {
            int emptyTileCount = 0;
            foreach (var tile in Tiles)
            {
                if (!tile.HaveObject())
                    emptyTileCount++;
            }

            if (emptyTileCount >= 30)
            {
                data.GeneratePlantsCountRange.x = 5;
                data.GeneratePlantsCountRange.y = 5;
            }
        }

        public void OnGetBalanceModifiers(GetBalanceModifiersEvent eventData)
        {
            eventData.Modifiers.Add(this);
        }
        #endregion

        private void OnEnable()
        {
            EventsManager.Subscribe<ReviveEvent>(OnRevive);
            EventsManager.Subscribe<CheckGameCompleteEvent>(OnCheckGameComplete);
            EventsManager.Subscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Subscribe<SaveEvent>(OnSave);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Subscribe<GetBalanceModifiersEvent>(OnGetBalanceModifiers);
            EventsManager.Subscribe<GetBoardObjectCountWithLayerEvent>(OnGetBoardObjectCountWithLayer);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<ReviveEvent>(OnRevive);
            EventsManager.Unsubscribe<CheckGameCompleteEvent>(OnCheckGameComplete);
            EventsManager.Unsubscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Unsubscribe<SaveEvent>(OnSave);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Unsubscribe<GetBoardObjectCountWithLayerEvent>(OnGetBoardObjectCountWithLayer);
        }
    }
}