using System;
using BloomLines.Assets;
using BloomLines.Managers;
using BloomLines.Saving;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BloomLines.Boards
{
    public class BoardTile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _highlight; // Зеленая подсветка тайла
        [SerializeField] private GameObject _dotHighlight; // Точка внутри тайла
        [SerializeField] private AnimationCurve _highlightScaleCurve;

        private BoardObjectBase _object;
        private RectTransform _rectTransform;

        public int Index { get; private set; }
        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public bool IsHighlight
        {
            set
            {
                _isHighlight = value;
                _highlight.SetActive(value);

                if (_object != null)
                    _object.transform.DOScale((_isHighlight || _isDotHighlight) ? 1.07f : 1f, 0.4f).SetEase(_highlightScaleCurve);
            }
        }
        public bool IsDotHighlight
        {
            set
            {
                _isDotHighlight = value;
                _dotHighlight.SetActive(value);

                if (_object != null)
                    _object.transform.DOScale((_isHighlight || _isDotHighlight) ? 1.07f : 1f, 0.4f).SetEase(_highlightScaleCurve);
            }
        }

        private bool _isHighlight;
        private bool _isDotHighlight;
        public Action<BoardTile> OnClick;
        public Action<BoardTile> OnPointerEnterEvent;
        public Action<BoardTile> OnPointerExitEvent;
        public BoardObjectBase Object => _object;
        public RectTransform RectTransform => _rectTransform;

        public void Initialize(int index, int tileX, int tileY)
        {
            Index = index;
            TileX = tileX;
            TileY = tileY;

            _rectTransform = GetComponent<RectTransform>();
        }

        // Еволюционируем обьект внутри тайла
        public void Evolve()
        {
            if (_object != null)
            {
                _object.Evolve();
            }
        }

        // Проверяем есть ли обьект у тайла
        public bool HaveObject()
        {
            return _object != null;
        }

        // Удаляем обьект с тайла
        public void RemoveObject(bool disableObject)
        {
            if (!HaveObject())
                return;

            if(disableObject)
                _object.gameObject.SetActive(false);

            _object = null;
        }

        // Ставим обьект в тайл
        public void PlaceObject(BoardObjectBase boardObject)
        {
            if (HaveObject())
                return;

            _object = boardObject;
            _object.gameObject.SetActive(true);
            _object.RectTransform.SetParent(_rectTransform);
            _object.RectTransform.offsetMin = Vector3.zero;
            _object.RectTransform.offsetMax = Vector3.zero;
            _object.RectTransform.localScale = Vector3.one;
            _object.RectTransform.localPosition = Vector3.zero;

            _object.SetTile(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                OnClick?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke(this);
        }

        // Проверяем можно ли использовать конкретный инструмент на этом тайле
        public bool CanUseTool(ToolType type)
        {
            if (type == ToolType.Pitchfork || type == ToolType.Rake || type == ToolType.Shovel) // Лопату, грабли, вилы, можно использовать на любом тайле
                return true;

            return HaveObject() && Object.CanUseTool(type); // Если другой инструмент то проверяем можно ли его использовать на предмете что внутри тайла
        }

        // Сохраняем состояние тайла
        public void Save()
        {
            var gameModeState = SaveManager.GameModeState;
            var boardState = gameModeState.BoardState;
            var tile = boardState.Tiles[Index];

            if (HaveObject())
            {
                if(tile.ObjectState == null)
                    tile.ObjectState = new TileObjectState();

                tile.ObjectState.Id = _object.Data.Id; // Айди обьекта в тайле
                tile.ObjectState.Data = _object.GetSaveData(); // Данные обьекта в тайле
            }
            else
            {
                tile.ObjectState = null;
            }
        }
    }
}