using System;
using BloomLines.Assets;
using BloomLines.Controllers;
using BloomLines.Managers;
using BloomLines.Saving;
using BloomLines.Tasks;
using BloomLines.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class UITaskTab : MonoBehaviour
    {
        [SerializeField] private RectTransform _goalsTab;
        [SerializeField] private Button _tabButton;
        [SerializeField] private GameObject _iconFill;
        [SerializeField] private GameObject _iconWarning;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _timeTab;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private UITaskGoal[] _goals;
        [SerializeField] private Sprite[] _goalIcons;

        private RectTransform _rectTransform;
        private TaskState _state;
        private TaskBase _task;
        private float _time;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _tabButton.onClick.AddListener(OnClick);
        }

        private void Update()
        {
            if (_state != null && _state.StateType == TaskStateType.Started)
            {
                if (_time > 0f)
                {
                    _time -= Time.deltaTime;
                    _timeText.text = GetTimeText(_time);

                    if(_time <= 0f)
                        EventsManager.Publish(new LoseTaskEvent());
                }
            }
        }

        private void OnClick()
        {
            if (_state.StateType == TaskStateType.Started)
                return;

            ChangeState(_state.StateType == TaskStateType.Unactive ? TaskStateType.Taken : TaskStateType.Unactive, true);

            AnalyticsController.SendEvent("click_task_tab");
        }

        public void StartTask()
        {
            ChangeState(TaskStateType.Started, true);
        }

        private void ChangeState(TaskStateType stateType, bool animated)
        {
            EventsManager.Publish(new ReloadBenchesEvent());

            _state.StateType = stateType;

            var gameModeState = SaveManager.GameModeState;
            float xPos = 0f;

            if (stateType == TaskStateType.Unactive)
                xPos = 125f;
            else if (stateType == TaskStateType.Taken)
            {
                var goalsCount = _state.GetGoalsCount();
                xPos = -107 * Mathf.Clamp(goalsCount - 1, 0, int.MaxValue);
            }

            if (stateType == TaskStateType.Unactive)
            {
                EventsManager.Publish(new SetConnectionTypeEvent(gameModeState.ConnectionType));
            }
            else
            {
                var currentGoalInfo = _task.GetCurrentGoalInfo();
                if(currentGoalInfo != null)
                    EventsManager.Publish(new SetConnectionTypeEvent(currentGoalInfo.ConnectionType));
            }

            _iconWarning.SetActive(stateType == TaskStateType.Unactive);
            _iconFill.SetActive(stateType != TaskStateType.Unactive);

            if (animated)
            {
                _goalsTab.DOAnchorPosX(xPos, 0.5f).SetEase(Ease.InOutSine);

                if (stateType == TaskStateType.Taken)
                    AudioController.Play("expand_menu");
                else if(stateType == TaskStateType.Unactive)
                    AudioController.Play("collapse_menu");
            }
            else
            {
                var goalsTabPos = _goalsTab.anchoredPosition;
                goalsTabPos.x = xPos;

                _goalsTab.anchoredPosition = goalsTabPos;
            }
        }

        public void UpdateVisual()
        {
            if (_state != null && !string.IsNullOrEmpty(_state.Id) && _state.StateType == TaskStateType.Started)
            {
                var gameModeState = SaveManager.GameModeState;
                var goalsInfo = _task.GetCurrentGoalInfo();

                if (goalsInfo != null)
                {
                    _goals[0].gameObject.SetActive(true);
                    _goals[0].Icon = _goalIcons[(int)goalsInfo.ConnectionType];
                    _goals[0].Count = Mathf.Clamp(goalsInfo.TargetValue, 0, int.MaxValue);
                }
            }
        }

        public void Show(TaskState state, TaskBase taskBase)
        {
            _state = state;
            _task = taskBase;

            gameObject.SetActive(true);

            var isTimed = state.IsTimed();

            foreach (var goal in _goals)
                goal.gameObject.SetActive(false);

            var goalsInfo = _task.GetAllGoalsInfo();

            for(int i = 0; i < goalsInfo.Length; i++)
            {
                _goals[i].gameObject.SetActive(true);
                _goals[i].Icon = _goalIcons[(int)goalsInfo[i].ConnectionType];
                _goals[i].Count = goalsInfo[i].TargetValue;
            }

            ChangeState(state.StateType, false);

            _rectTransform.DOAnchorPosX(2f, 0.5f).SetEase(Ease.InOutSine);
            _canvasGroup.interactable = true;

            int reward = state.GetReward();
            _time = state.GetTime();

            _rewardText.text = reward.ToString();
            _timeTab.SetActive(isTimed);
            if (isTimed)
                _timeText.text = GetTimeText(_time);

            UpdateVisual();
        }

        private string GetTimeText(float time)
        {
            time = Mathf.Clamp(time, 0f, float.MaxValue);

            int totalSeconds = Mathf.FloorToInt(time);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:00}:{seconds:00}";
        }

        public void Hide()
        {
            _canvasGroup.interactable = false;

            DOTween.Sequence()
                .Append(_rectTransform.DOAnchorPosX(185f, 0.5f).SetEase(Ease.InOutSine))
                .Join(_goalsTab.DOAnchorPosX(125f, 0.5f).SetEase(Ease.InOutSine))
                .AppendCallback(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        private void OnCollectItemToBench(CollectItemToBenchEvent eventData)
        {
            if (_task == null)
                return;

            _task.OnCollectItemToBench(eventData);

            UpdateVisual();

            if (!_task.IsCompleted())
            {
                var currentGoal = _task.GetCurrentGoalInfo();
                if (currentGoal != null)
                    EventsManager.Publish(new SetConnectionTypeEvent(currentGoal.ConnectionType));
            }
        }

        private void OnSave(SaveEvent eventData)
        {
            if (eventData.Phase != SavePhase.Prepare || eventData.Type != SaveType.GameMode)
                return;

            if (_state == null || string.IsNullOrEmpty(_state.Id) || !_state.IsTimed())
                return;

            var values = _state.Data.Split(';');
            if (values.Length >= 2)
            {
                _state.Data = string.Empty;
                for (int i = 0; i < values.Length - 1; i++)
                {
                    _state.Data += $";{values[i]}";
                }
            }

            _state.Data += $";{_time}";
            _state.Data = _state.Data.TrimStart(';');
        }

        private void OnEnable()
        {
            EventsManager.Subscribe<SaveEvent>(OnSave);
            EventsManager.Subscribe<CollectItemToBenchEvent>(OnCollectItemToBench);
        }

        private void OnDisable()
        {
            EventsManager.Unsubscribe<SaveEvent>(OnSave);
            EventsManager.Unsubscribe<CollectItemToBenchEvent>(OnCollectItemToBench);
        }
    }
}