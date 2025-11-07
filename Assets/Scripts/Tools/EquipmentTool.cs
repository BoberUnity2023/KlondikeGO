using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;

namespace BloomLines.Tools
{
    public class ResetToolEvent
    {
        private ToolType _toolType;

        public ToolType ToolType => _toolType;

        public ResetToolEvent(ToolType toolType)
        {
            _toolType = toolType;
        }
    }

    public abstract class EquipmentTool : MonoBehaviour
    {
        [SerializeField] protected ToolType _toolType;
        [SerializeField] protected Board _board;
        [SerializeField] protected RectTransform _icon;

        protected Animator _anim;
        protected RectTransform _rectTransform;
        protected Camera _camera;
        protected Tween _shakeTween;

        public ToolType ToolType => _toolType;
        public bool IsActive { get; protected set; }

        private void Awake()
        {
            _camera = Camera.main;
            _anim = GetComponent<Animator>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (IsActive)
            {
                _rectTransform.sizeDelta = _board.Tiles[0].RectTransform.sizeDelta;

                var position = _camera.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0f;

                _rectTransform.position = position;
            }
        }

        public void DisableObject()
        {
            gameObject.SetActive(false);
        }

        protected abstract void UseDelay();

        public virtual void Use(BoardTile tile)
        {
            if (!IsActive)
                return;

            if (tile.CanUseTool(_toolType))
            {
                _shakeTween?.Complete(true);
                _shakeTween = null;

                IsActive = false;
                _anim.SetTrigger("Use");

                Cursor.Cursor.ReleaseTool();

                EventsManager.Publish(new ResetToolEvent(_toolType));
            }
            else
            {
                AudioController.Play("wrong_use_tool");

                _anim.enabled = false;
                _shakeTween = DOTween.Sequence().Append(_icon.DOShakeAnchorPos(0.15f, 8f, 25, 90, false, true, ShakeRandomnessMode.Harmonic)).AppendCallback(() =>
                {
                    _anim.enabled = true;
                });
            }
        }

        public virtual void Equip()
        {
            IsActive = true;
            gameObject.SetActive(true); 
        }

        public virtual void Release()
        {
            if (!IsActive)
                return;

            _shakeTween?.Complete(true);
            _shakeTween = null;

            IsActive = false;
            _anim.SetTrigger("Hide");
        }

        private void OnEnable()
        {
            Update();
        }
    }
}