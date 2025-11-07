using System.Linq;
using BloomLines.Cursor;
using BloomLines.Managers;
using BloomLines.Tools;
using UnityEngine;

namespace BloomLines.Controllers
{
    public class ToolsController : MonoBehaviour
    {
        [SerializeField] private EquipmentTool[] _tools; // Все инструменты

        // Когда используем инструмент
        private void OnUseTool(UseToolEvent eventData)
        {
            var tool = _tools.FirstOrDefault(e => e.ToolType == eventData.Type);
            tool.Use(eventData.Tile);
        }

        // Берем в руки инструмент
        private void OnEquipTool(EquipToolEvent eventData)
        {
            var tool = _tools.FirstOrDefault(e => e.ToolType == eventData.Type);
            tool.Equip();
        }

        // Убираем из рук инструмент
        private void OnReleaseTool(ReleaseToolEvent eventData)
        {
            var tool = _tools.FirstOrDefault(e => e.ToolType == eventData.Type);
            tool.Release();
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<UseToolEvent>(OnUseTool);
            EventsManager.Subscribe<EquipToolEvent>(OnEquipTool);
            EventsManager.Subscribe<ReleaseToolEvent>(OnReleaseTool);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<UseToolEvent>(OnUseTool);
            EventsManager.Unsubscribe<EquipToolEvent>(OnEquipTool);
            EventsManager.Unsubscribe<ReleaseToolEvent>(OnReleaseTool);
        }
    }
}