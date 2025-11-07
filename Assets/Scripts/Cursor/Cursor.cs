using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Cursor
{
    // Ивент для взятия инструмента в руки
    public class EquipToolEvent
    {
        public ToolType Type { get; private set; }

        public EquipToolEvent(ToolType type)
        {
           Type = type;
        }
    }

    // Ивент для отпускания предмета из рук
    public class ReleaseToolEvent
    {
        public ToolType Type { get; private set; }

        public ReleaseToolEvent(ToolType type)
        {
            Type = type;
        }
    }

    // Ивент использования инструмента на тайле
    public class UseToolEvent
    {
        public BoardTile Tile { get; private set; }
        public ToolType Type { get; private set; }

        public UseToolEvent(BoardTile tile, ToolType type)
        {
            Tile = tile;
            Type = type;
        }
    }

    public static class Cursor
    {
        private static ToolType _toolType;

        public static ToolType EquipmentTool => _toolType;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _toolType = ToolType.None;

            var updateHelper = new GameObject("CursorUpdater").AddComponent<UpdateHelper>();
            updateHelper.OnUpdate += Update;

            GameObject.DontDestroyOnLoad(updateHelper);
        }

        private static void Update()
        {
            if (!TutorialController.IsCompleted(TutorialIds.FIRST_GAME))
                return;

            if (_toolType != ToolType.None)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                {
                    ReleaseTool();
                }
            }
        }

        // Используем инструмент
        public static void Use(BoardTile tile)
        {
            if (_toolType == ToolType.None)
                return;

            EventsManager.Publish(new UseToolEvent(tile, _toolType));
        }

        // Берем инструмент в руки
        public static void EquipTool(ToolType type)
        {
            if (_toolType != ToolType.None)
                ReleaseTool();

            _toolType = type;
            EventsManager.Publish(new EquipToolEvent(type));

            UnityEngine.Cursor.visible = false;
        }

        // Отпускаем инструмент из рук
        public static void ReleaseTool()
        {
            if (_toolType == ToolType.None)
                return;

            EventsManager.Publish(new ReleaseToolEvent(_toolType));
            _toolType = ToolType.None;

            UnityEngine.Cursor.visible = true;
        }
    }
}