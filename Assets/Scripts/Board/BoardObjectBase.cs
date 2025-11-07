using BloomLines.Assets;
using BloomLines.ObjectPool;
using BloomLines.Saving;
using UnityEngine;

namespace BloomLines.Boards
{
    // Стандартный класс от которого наследуются все обьекты которые на доске
    public abstract class BoardObjectBase : PoolObject, ISaveable
    {
        [SerializeField] protected BoardObjectData _data; // Данные этого обьекта
        [SerializeField] protected CanvasGroup _canvasGroup;

        protected BoardTile _targetTile;
        protected RectTransform _rectTransform;
        protected bool _isReadyToDestroy;

        public bool IsReadyToDestroy // Готовится ли обьект к разрушению, чтобы не использовать повторно
        {
            get { return _isReadyToDestroy; }
            set { _isReadyToDestroy = value; }
        }
        public RectTransform RectTransform => _rectTransform;
        public BoardObjectData Data => _data;
        public CanvasGroup CanvasGroup => _canvasGroup;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        // Выставляем тайл на котором стоит обьект
        public void SetTile(BoardTile tile)
        {
            _targetTile = tile;
        }

        // Можно ли использовать конкретный тип инструмента на этом обьекте
        public abstract bool CanUseTool(ToolType type);

        public virtual void OnSpawn()
        {
            _isReadyToDestroy = false;
        }

        public abstract void ResetEvolve(); // Сбросить обьект
        public abstract void Evolve(); // Еволюционировать обьект
        public abstract bool CanMove(); // Можно ли двигать обьект
        public abstract bool CanReplace(); // Можно ли переставить обьект
        public abstract bool CanMoveThrough(); // Можно ли проходить сквозь обьект

        #region ISaveable
        public virtual string GetSaveData()
        {
            return string.Empty;
        }

        public virtual void LoadSaveData(string data)
        {

        }
        #endregion
    }
}