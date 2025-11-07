using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.UI
{
    public class OpenPanelEvent
    {
        private UIPanelType _panelType;

        public UIPanelType PanelType => _panelType;

        public OpenPanelEvent(UIPanelType panelType)
        {
            _panelType = panelType;
        }
    }

    public class ClosePanelEvent
    {
        private UIPanelType _panelType;

        public UIPanelType PanelType => _panelType;

        public ClosePanelEvent(UIPanelType panelType)
        {
            _panelType = panelType;
        }
    }

    public enum UIPanelType
    {
        None,
        Settings,
        Shop,
        GamemodeChoice,
        Sounds,
        SelectLanguage,
        Result,
        DailyReward,
        Leaderboard,
        RateGame,
    }

    [RequireComponent(typeof(Animator))]
    public abstract class UIPanelBase : MonoBehaviour
    {
        [SerializeField] private UIPanelType _panelType;

        private Animator _animator;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        protected virtual void Open()
        {
            AudioController.Play("open_menu");

            gameObject.SetActive(true);
            _animator.SetTrigger("Show");
        }

        protected virtual void Close()
        {
            _animator.SetTrigger("Close");
        }

        private void OnOpenPanel(OpenPanelEvent eventData)
        {
            if (_panelType != eventData.PanelType)
                return;

            Open();
        }

        private void OnClosePanel(ClosePanelEvent eventData)
        {
            if (_panelType != eventData.PanelType)
                return;

            Close();
        }

        protected virtual void OnEnable()
        {
            EventsManager.Subscribe<OpenPanelEvent>(OnOpenPanel);
            EventsManager.Subscribe<ClosePanelEvent>(OnClosePanel);
        }

        protected virtual void OnDisable()
        {
            EventsManager.Unsubscribe<OpenPanelEvent>(OnOpenPanel);
            EventsManager.Unsubscribe<ClosePanelEvent>(OnClosePanel);
        }
    }
}