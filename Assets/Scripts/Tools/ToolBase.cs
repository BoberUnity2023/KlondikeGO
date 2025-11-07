using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Cursor;
using BloomLines.Managers;
using BloomLines.Saving;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Tools
{
    public class UpdateToolsEvent { }

    public abstract class ToolBase : MonoBehaviour, ISaveable, IBalanceModifier
    {
        [SerializeField] protected Button _button;
        [SerializeField] protected ToolData _data;
        [SerializeField] protected Image _icon;

        protected Animator _anim;

        protected bool _isActive;
        public Image Icon => _icon;

        private void Awake()
        {
            _anim = GetComponent<Animator>();

            _button.onClick.AddListener(OnClick);
        }

        protected virtual void OnClick()
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null && gameModeState.Type == GameModeType.Classic)
            {
                EventsManager.Publish(new ShowNotificationEvent("wrong_gamemode", string.Empty));
                return;
            }

            if (!_isActive)
                return;

            if (Cursor.Cursor.EquipmentTool == _data.ToolType)
            {
                Cursor.Cursor.ReleaseTool();
            }
            else
            {
                AudioController.Play("equip_tool");

                _anim.SetBool("OnTake", true);
                Cursor.Cursor.EquipTool(_data.ToolType);

                AnalyticsController.SendEvent($"equip_tool_{_data.ToolType.ToString().ToLowerInvariant()}");
            }
        }

        public virtual void UpdateVisual(bool withAnimation)
        {

        }

        public virtual string GetSaveData()
        {
            return string.Empty;
        }

        protected virtual void OnStartGameMode(StartGameModeEvent eventData)
        {
            var toolState = eventData.State.GetToolState(_data.ToolType);
            LoadSaveData(toolState.Data);
        }

        protected virtual void OnResetTool(ResetToolEvent eventData)
        {

        }

        protected  void OnReleaseTool(ReleaseToolEvent eventData)
        {
            if (eventData.Type != _data.ToolType)
                return;

            _anim.SetBool("OnTake", false);
        }

        private void OnUpdateTools(UpdateToolsEvent eventData)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState != null)
            {
                var toolState = gameModeState.GetToolState(_data.ToolType);
                LoadSaveData(toolState.Data);
            }
        }

        #region IBalanceModifier
        public int Priority => 1;
        public virtual void Apply(BalanceData data)
        {
        }

        public void OnGetBalanceModifiers(GetBalanceModifiersEvent eventData)
        {
            eventData.Modifiers.Add(this);
        }
        #endregion

        #region ISaveable
        public virtual void LoadSaveData(string data)
        {
            UpdateVisual(false);
        }

        protected virtual void OnSave(SaveEvent eventData)
        {

        }
        #endregion

        protected virtual void OnEnable()
        {
            EventsManager.Subscribe<UpdateToolsEvent>(OnUpdateTools);
            EventsManager.Subscribe<ResetToolEvent>(OnResetTool);
            EventsManager.Subscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Subscribe<SaveEvent>(OnSave);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Subscribe<GetBalanceModifiersEvent>(OnGetBalanceModifiers);
        }

        protected virtual void OnDisable()
        {
            EventsManager.Unsubscribe<UpdateToolsEvent>(OnUpdateTools);
            EventsManager.Unsubscribe<ResetToolEvent>(OnResetTool);
            EventsManager.Unsubscribe<ReleaseToolEvent>(OnReleaseTool);
            EventsManager.Unsubscribe<SaveEvent>(OnSave);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Unsubscribe<GetBalanceModifiersEvent>(OnGetBalanceModifiers);
        }
    }
}