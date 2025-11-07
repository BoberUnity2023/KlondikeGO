using System;
using System.Collections;
using BloomLines.Adaptation;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class ChangeConnectionTypeEvent
    {
        private ConnectionType _connectionType;

        public ConnectionType ConnectionType => _connectionType;

        public ChangeConnectionTypeEvent(ConnectionType connectionType)
        {
            _connectionType = connectionType;
        }
    }

    public class SetConnectionTypeEvent
    {
        private ConnectionType _connectionType;

        public ConnectionType ConnectionType => _connectionType;

        public SetConnectionTypeEvent(ConnectionType connectionType)
        {
            _connectionType = connectionType;
        }
    }

    public class UIConnectionTypeTab : AdaptationObjectBase, IBalanceModifier
    {
        [SerializeField] private Button _arrowButton;
        [SerializeField] private Sprite _activeArrowIcon;
        [SerializeField] private Sprite _unactiveArrowIcon;
        [SerializeField] private Image _shadow;
        [SerializeField] private RectTransform _selectionTabContent;
        [SerializeField] private UIConnectionTypeButton _currentTypeButton;
        [SerializeField] private UIConnectionTypeButton[] _otherTypeButtons;
        [SerializeField] private float _animationTime;
        [SerializeField] private AnimationCurve _arrowButtonCurve;
        [SerializeField] private ParticleSystem _highlightVfx;

        private bool _isOpen;
        private Tween _selectionTabTween;

        private void Awake()
        {
            _arrowButton.onClick.AddListener(() => TrySwitch());
            _currentTypeButton.Initialize((connectionType) => TrySwitch());

            foreach (var btn in _otherTypeButtons)
            {
                btn.Initialize((type) =>
                {
                    OnSelectConnectionType(type, true, false);

                    AnalyticsController.SendEvent($"select_connection_type_{type.ToString().ToLowerInvariant()}");
                });
            }

            SetSelectionState(false, false);
        }

        private void TrySwitch()
        {
            var gameModeState = SaveManager.GameModeState;

            if (_currentTypeButton.IsActive)
                SetSelectionState(!_isOpen, true);
            else if(gameModeState != null && gameModeState.Type == GameModeType.Classic)
                EventsManager.Publish(new ShowNotificationEvent("wrong_gamemode", string.Empty));

            AnalyticsController.SendEvent("click_select_connection_tab");
        }

        private IEnumerator TryCloseInClickCoroutine()
        {
            var waitFrame = new WaitForEndOfFrame();
            for (int i = 0; i < 3; i++)
                yield return waitFrame;

            while (true)
            {
                yield return null;

                if (!_isOpen)
                    break;

                if (_isOpen && Input.GetMouseButtonUp(0))
                {
                    SetSelectionState(false, true);
                }
            }
        }

        private void OnSelectConnectionType(ConnectionType connectionType, bool animatedClose, bool gameLoaded)
        {
            if (!_isOpen)
                return;

            var gameModeState = SaveManager.GameModeState;
            bool isLine4 = gameModeState.ConnectionType == ConnectionType.Line4;
          
            if (!isLine4 && connectionType == ConnectionType.Line4 && gameModeState.MovesCountAfterSell > 0)
            {
                StopAllCoroutines();
                SetSelectionState(_isOpen, false);
                return;
            }

            if (!gameLoaded)
            {
                gameModeState.MovesAfterSelectConnectionType = 0;

                if (gameModeState.MovesCountAfterSell > 0 && (gameModeState.ConnectionType == ConnectionType.Line4 || connectionType == ConnectionType.Line4))
                {
                    return;
                }
            }

            _currentTypeButton.SetConnectionType(connectionType);

            int index = 0;
            for(int i = 0; i < Enum.GetValues(typeof(ConnectionType)).Length; i++)
            {
                var type = (ConnectionType)i;
                if (type == connectionType)
                    continue;

                _otherTypeButtons[index].SetConnectionType(type);
                index++;
            }

            gameModeState.ConnectionType = connectionType;

            SetSelectionState(false, animatedClose);
            UpdateButtons();

            EventsManager.Publish(new ChangeConnectionTypeEvent(connectionType));
        }

        private void SetSelectionState(bool isOpen, bool animated)
        {
            _isOpen = isOpen;

            _selectionTabTween?.Kill();

            var arrowButtonTr = _arrowButton.image.rectTransform;
            var arrowButtonRotation = Vector3.forward * (isOpen ? 180f : 0f);
            var posY = isOpen ? -3f : (_selectionTabContent.sizeDelta.y * 1.05f);

            if (animated)
            {
                _selectionTabTween = DOTween.Sequence()
                    .Append(_selectionTabContent.DOAnchorPosY(posY, _animationTime).SetEase(Ease.OutCubic))
                    .Join(arrowButtonTr.DOLocalRotate(arrowButtonRotation, _animationTime * 1.2f).SetEase(_arrowButtonCurve));

                if (isOpen)
                    AudioController.Play("expand_menu");
                else
                    AudioController.Play("collapse_menu");
            }
            else
            {
                var pos = _selectionTabContent.anchoredPosition;
                pos.y = posY;

                _selectionTabContent.anchoredPosition = pos;
                arrowButtonTr.localEulerAngles = arrowButtonRotation;
            }

            if (isOpen)
                StartCoroutine(TryCloseInClickCoroutine());
            else
                StopAllCoroutines();
        }

        protected override void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            var posY = _isOpen ? -3f : (_selectionTabContent.sizeDelta.y * 1.05f);
            var pos = _selectionTabContent.anchoredPosition;
            pos.y = posY;

            _selectionTabContent.anchoredPosition = pos;
        }

        protected override void OnScreenTypeChanged(ScreenType screenType)
        {
        }

        private void UpdateButtons()
        {
            var gameModeState = SaveManager.GameModeState;
            bool isClassic = gameModeState.Type == GameModeType.Classic;
            bool isLine4 = gameModeState.ConnectionType == ConnectionType.Line4;
            bool active = gameModeState.MovesCountAfterSell <= 0 && gameModeState.Type != GameModeType.Classic;
            bool takenOrActiveTask = gameModeState.TaskState != null && gameModeState.TaskState.StateType != TaskStateType.Unactive;

            if (!isClassic && !isLine4 && (gameModeState.MovesAfterSelectConnectionType == 0 || gameModeState.MovesAfterSelectConnectionType >= 7))
                active = true;

            if (!TutorialController.IsCompleted(TutorialIds.FIRST_GAME))
                active = true;

            if (!isLine4 && active && !takenOrActiveTask)
                _highlightVfx.Play();
            else
                _highlightVfx.Stop();

            _otherTypeButtons[0].IsActive = isLine4 || gameModeState.MovesCountAfterSell <= 0;

            bool isArrowActive = active && !takenOrActiveTask;
            _arrowButton.image.sprite = isArrowActive ? _activeArrowIcon : _unactiveArrowIcon;
            //_arrowButton.interactable = (active && !takenOrActiveTask);
            _currentTypeButton.IsActive = active && !takenOrActiveTask;

            _shadow.gameObject.SetActive(!isArrowActive);
            int moves = Mathf.Clamp(gameModeState.MovesAfterSelectConnectionType, 0, 7);
            _shadow.fillAmount = (7f - moves) / 7;            
        }

        private void OnStartGameMode(StartGameModeEvent eventData)
        {
            _isOpen = true;
            OnSelectConnectionType(eventData.State.ConnectionType, false, true);

            UpdateButtons();
        }

        private void OnSellItems(SellItemsEvent eventData)
        {
            UpdateButtons();
        }

        private void OnMakeMove(MakeMoveEvent eventData)
        {
            UpdateButtons();
        }

        private void OnSetConnectionType(SetConnectionTypeEvent eventData)
        {
            _isOpen = true;
            OnSelectConnectionType(eventData.ConnectionType, false, false);
            _isOpen = false;
        }

        #region IBalanceModifier
        public int Priority => 10;
        public void Apply(BalanceData data)
        {
            var gameModeState = SaveManager.GameModeState;
            if (gameModeState == null)
                return;

            bool isLine4 = gameModeState.ConnectionType == ConnectionType.Line4;

            float rockChance = isLine4 ? 0.06f : 0.04f;
            float webChance = isLine4 ? 0.45f : 0.20f;
            float moleChance = isLine4 ? 0.14f : 0.08f;

            if (gameModeState.Score >= 500 && gameModeState.Score < 1000)
            {
                rockChance += isLine4 ? 0.02f : 0.01f;
                webChance += isLine4 ? 0.15f : 0.05f;
                moleChance += isLine4 ? 0.06f : 0.02f;
            }
            else if (gameModeState.Score >= 1000 && gameModeState.Score < 1500)
            {
                rockChance += isLine4 ? 0.06f : 0.02f;
                webChance += isLine4 ? 0.25f : 0.10f;
                moleChance += isLine4 ? 0.11f : 0.04f;
            }
            else if (gameModeState.Score >= 1500)
            {
                rockChance += isLine4 ? 0.08f : 0.03f;
                webChance += isLine4 ? 0.35f : 0.15f;
                moleChance += isLine4 ? 0.16f : 0.06f;
            }

            data.ChanceToSpawnRock = rockChance;
            data.ChanceToSpawnWeb = webChance;
            data.ChanceToSpawnMole = moleChance;
        }

        public void OnGetBalanceModifiers(GetBalanceModifiersEvent eventData)
        {
            eventData.Modifiers.Add(this);
        }
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            EventsManager.Subscribe<SellItemsEvent>(OnSellItems);
            EventsManager.Subscribe<MakeMoveEvent>(OnMakeMove);
            EventsManager.Subscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Subscribe<SetConnectionTypeEvent>(OnSetConnectionType);
            EventsManager.Subscribe<GetBalanceModifiersEvent>(OnGetBalanceModifiers);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventsManager.Unsubscribe<SellItemsEvent>(OnSellItems);
            EventsManager.Unsubscribe<MakeMoveEvent>(OnMakeMove);
            EventsManager.Unsubscribe<StartGameModeEvent>(OnStartGameMode);
            EventsManager.Unsubscribe<SetConnectionTypeEvent>(OnSetConnectionType);
            EventsManager.Unsubscribe<GetBalanceModifiersEvent>(OnGetBalanceModifiers);
        }
    }
}