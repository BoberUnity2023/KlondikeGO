using System.Collections.Generic;
using BloomLines.Adaptation;
using BloomLines.Assets;
using BloomLines.Managers;
using UnityEngine;

namespace BloomLines.Boards
{
    [RequireComponent(typeof(ScreenChangeNotifier))]
    public class Bench : MonoBehaviour
    {
        [SerializeField] private ScreenChangeNotifier _notifier; // Подписываем на события обновления экрана
        [SerializeField] private RectTransform _firstPoint;
        [SerializeField] private RectTransform _secondPoint;

        private int _maxItemsCount;
        private Board _board;
        private List<SellItem> _items = new List<SellItem>();

        public List<SellItem> Items => _items;
        public int ItemsCount => _items.Count;
        public bool HaveFreeSpace => _items.Count < _maxItemsCount;

        public void Initialize(Board board)
        {
            _board = board;
        }

        // Выставляем максимальное количество предметов которое может быть на лавочке
        public void SetMaxItemsCount(int maxItemsCount)
        {
            _maxItemsCount = maxItemsCount;
        }

        // Очищаем все предметы с лавочки
        public void Clear()
        {
            foreach (var item in _items)
                item.gameObject.SetActive(false);

            _items.Clear();
        }

        // Ставим предмет для продажи на лавочку
        public SellItem Add(SellItemData itemData)
        {
            if (!HaveFreeSpace)
                return null;

            // Спавним предмет
            var sellItem = SpawnerManager.SellItemSpawner.SpawnObject();
            sellItem.RectTransform.SetParent(transform);

            if (!sellItem.IsInitialized)
                sellItem.Initialize(_board);

            // Выставляем рандомный поворот и нужный размер
            var rotation = itemData.ConnectionType == ConnectionType.Line4 ? Vector3.forward * Random.Range(-15f, 15f) : Vector3.zero;
            var scale = itemData.ConnectionType == ConnectionType.Line4 ? 1f : 1.3f;

            sellItem.Set(itemData);
            sellItem.RectTransform.localEulerAngles = rotation;
            sellItem.RectTransform.localScale = Vector3.one * scale;
            sellItem.RectTransform.position = GetItemPosition(_items.Count);

            _items.Add(sellItem);

            return sellItem; 
        }

        // Получает позицию определеного предмета на лавочке по очереди
        public Vector3 GetItemPosition(int itemCount)
        {
            var value = ((float)itemCount + 1f) / (_maxItemsCount + 1);
            var position = Vector3.Lerp(_firstPoint.position, _secondPoint.position, value);

            return position;
        }

        // Обновляем положения товаров на лавочке
        private void UpdateVisual()
        {
            for(int i = 0; i < _items.Count; i++)
            {
                var pos = GetItemPosition(i);
                _items[i].RectTransform.position = pos;
            }
        }

        private void OnScreenUpdate(ScreenInfoEvent eventData)
        {
            UpdateVisual();
        }

        private void OnEnable()
        {
            _notifier.OnScreenUpdate += OnScreenUpdate;
        }

        private void OnDisable()
        {
            _notifier.OnScreenUpdate -= OnScreenUpdate;
        }
    }
}