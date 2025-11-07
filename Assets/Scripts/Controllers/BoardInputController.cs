using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Cursor;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Controllers
{
    public class MakeMoveEvent { } // Ивент при выполнении хода игроком

    // Скрипт для управление обьектами на поле
    [RequireComponent(typeof(Board))]
    public class BoardInputController : MonoBehaviour
    {
        private int _interactRadius;
        private Board _board;
        private BoardTile _selectedTile;

        private void Awake()
        {
            _board = GetComponent<Board>();

            foreach (var tile in _board.Tiles) // Подписываем на события всех тайлов
            {
                tile.OnClick += OnClickTile;
                tile.OnPointerEnterEvent += OnPointerEnter;
                tile.OnPointerExitEvent += OnPointerExit;
            }
        }

        // Когда наводимся на тайл
        private void OnPointerEnter(BoardTile tile)
        {
            var tool = Cursor.Cursor.EquipmentTool;
            if (tool != ToolType.None) // Если есть инструмент в руках
            {
                tile.IsHighlight = true; // подсвечиваем тайл

                _interactRadius = -1; // радиус взаимодействия инструмента
                if (tool == ToolType.Pitchfork || tool == ToolType.Rake || tool == ToolType.Shovel)
                {
                    switch (tool)
                    {
                        case ToolType.Pitchfork:
                            _interactRadius = 1;
                            break;
                        case ToolType.Rake:
                            _interactRadius = 2;
                            break;
                        case ToolType.Shovel:
                            _interactRadius = 10;
                            break;
                    }

                    if (_interactRadius != -1)
                    {
                        var tiles = _board.GetAllTilesInRadius(tile.TileX, tile.TileY, _interactRadius); // Подсвечиваем все тайлы в радиусе взаимодействия
                        foreach (var t in tiles)
                            t.IsDotHighlight = true;
                    }
                }
            }
        }

        // Когда перестаем наводится на тайл
        private void OnPointerExit(BoardTile tile)
        {
            var tool = Cursor.Cursor.EquipmentTool;
            if (tool != ToolType.None) // Если есть инструмент в руках
            {
                if (tool != ToolType.Secateurs) // Если не ножницы в руках, убираем выделение
                {
                    tile.IsHighlight = false;
                }
                else // Если ножницы в руках
                {
                    // Убираем выделение только у обьектов которые НЕ СОРНЯКИ
                    if (!tile.HaveObject() || !(tile.Object is Plant) || (tile.Object as Plant).PlantData.PlantType != PlantType.Weed)
                        tile.IsHighlight = false;
                }

                if (_interactRadius != -1) // Убираем выделение в радиусе взаимодействия инструмента
                {
                    var tiles = _board.GetAllTilesInRadius(tile.TileX, tile.TileY, _interactRadius);
                    foreach (var t in tiles)
                        t.IsDotHighlight = false;

                    _interactRadius = -1;
                }
            }
        }

        // Когда кликаем на тайл
        private void OnClickTile(BoardTile tile)
        {
            if (Cursor.Cursor.EquipmentTool == Assets.ToolType.None)
                SimpleClickTile(tile);
            else
                ClickTileWithActiveTool(tile);
        }

        // Обычный клик на тайл
        private void SimpleClickTile(BoardTile tile)
        {
            #region Analytics
            if (tile.HaveObject())
            {
                switch (tile.Object.Data.ObjectType)
                {
                    case BoardObjectType.Plant:
                        var plant = tile.Object as Plant;
                        if (plant.HaveWeb)
                            AnalyticsController.SendEvent("click_web");
                        else
                            AnalyticsController.SendEvent("click_plant");
                        break;
                    case BoardObjectType.Rock:
                        AnalyticsController.SendEvent("click_rock");
                        break;
                    case BoardObjectType.Mole:
                        AnalyticsController.SendEvent("click_mole");
                        break;
                }
            }
            #endregion

            if (_selectedTile == null)
            {
                if (tile.HaveObject() && tile.Object.CanMove()) // Выделяем первый обьект который можно двигать
                {
                    _selectedTile = tile;
                    tile.IsHighlight = true;

                    AudioController.Play("select_plant");
                }
            }
            else
            {
                if (_selectedTile.Index != tile.Index) // Если кликнули на второй, другой тайл то переставляем обьекты
                {
                    var gameModeState = SaveManager.GameModeState;
                    gameModeState.MovesCountAfterSell++;
                    gameModeState.MovesAfterSelectConnectionType++;

                    EventsManager.Publish(new MakeMoveEvent());

                    if (_board.CanReplace(_selectedTile.Index, tile.Index))
                        AudioController.Play("place_plant");
                    else
                    {
                        AudioController.Play("wrong_use_tool");
                        var plant = _selectedTile.Object as Plant;
                        if (plant != null)
                            plant.WrongMove();
                    }

                    StartCoroutine(_board.ReplaceObjects(_selectedTile.Index, tile.Index));
                }

                DeselectTile();
            }
        }

        // Клик на тайл с инструментом в руках
        private void ClickTileWithActiveTool(BoardTile tile)
        {
            Cursor.Cursor.Use(tile); // Используем инструмент на тайле
        }

        // Снимаем выделение с тайла текущего
        private void DeselectTile()
        {
            if (_selectedTile != null)
            {
                _selectedTile.IsHighlight = false;
                _selectedTile = null;
            }
        }

        // Когда берем инструмент в руки
        private void OnEquipTool(EquipToolEvent eventData)
        {
            DeselectTile();

            if(eventData.Type == ToolType.Secateurs) // Если взяли ножницы то подсвечиваем все сорняки
            {
                foreach(var tile in _board.Tiles)
                {
                    if (tile.HaveObject() && tile.Object is Plant && (tile.Object as Plant).PlantData.PlantType == PlantType.Weed)
                        tile.IsHighlight = true;
                }
            }

            if (eventData.Type == ToolType.Pickaxe) // Если взяли кирку то подсвечиваем каменные цветы
            {
                foreach (var tile in _board.Tiles)
                {
                    var plant = tile.Object as Plant;
                    if (plant != null)
                    {
                        var data = plant.PlantData;
                        if (data != null)
                        {                            
                            if (tile.HaveObject() && tile.Object is Plant && data.PlantType == PlantType.Flower_Crystal && tile.CanUseTool(ToolType.Pickaxe))
                                tile.IsHighlight = true;
                        }
                    }

                    var rock = tile.Object as Rock; //и камни
                    if (rock != null)
                    {
                        var data = rock.Data;
                        if (data != null)
                        {
                            if (tile.HaveObject() && tile.Object is Rock)
                                tile.IsHighlight = true;
                        }
                    }
                }
            }
        }

        // Когда отпускаем инструмент из рук
        private void OnReleaseTool(ReleaseToolEvent eventData)
        {
            DeselectTile();
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            DeselectTile();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<EquipToolEvent>(OnEquipTool);
            EventsManager.Subscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<EquipToolEvent>(OnEquipTool);
            EventsManager.Unsubscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
        }
    }
}