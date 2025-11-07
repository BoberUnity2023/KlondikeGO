using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Controllers;
using BloomLines.Helpers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;

namespace BloomLines.Tools
{
    public class Pickaxe : EquipmentTool
    {
        [SerializeField] private GameObject _destroyRockVFX;

        private BoardTile _tile;

        public override void Use(BoardTile tile)
        {
            base.Use(tile);

            if (tile.CanUseTool(_toolType))
            {
                _tile = tile;

                #region Analytics
                if (tile.HaveObject())
                {
                    switch (tile.Object.Data.ObjectType)
                    {
                        case BoardObjectType.Plant:
                            var plant = tile.Object as Plant;
                            if (plant.HaveWeb)
                                AnalyticsController.SendEvent("pickaxe_click_web");
                            else if (plant.PlantData.PlantType == PlantType.Flower_Crystal)
                                AnalyticsController.SendEvent("pickaxe_click_crystal_plant");
                            break;
                        case BoardObjectType.Rock:
                            AnalyticsController.SendEvent("pickaxe_click_rock");
                            break;
                    }
                }
                #endregion
            }
        }

        protected override void UseDelay()
        {
            if (_tile == null)
                return;

            Vibration.Vibrate(30);

            var obj = _tile.Object;

            var sequence = DOTween.Sequence();

            _tile.RemoveObject(false);

            switch (obj.Data.ObjectType)
            {
                case BoardObjectType.Plant:
                    AudioController.Play("break_crystal");
                    sequence.Append(obj.RectTransform.DOScale(0f, 0.3f).SetEase(Ease.InSine));

                    EventsManager.Publish(new CutPlantEvent(_tile, obj as Plant));
                    break;
                case BoardObjectType.Rock:
                    AudioController.Play("break_rock");
                    _destroyRockVFX.SetActive(true);

                    sequence
                        .Append(obj.RectTransform.DOShakeAnchorPos(0.6f, 3f, 20, 90, false, true, ShakeRandomnessMode.Harmonic).SetEase(Ease.OutSine))
                        .Join(obj.RectTransform.DOScale(0f, 0.6f).SetEase(Ease.OutSine))
                        .Join(obj.RectTransform.DOAnchorPosY(obj.RectTransform.anchoredPosition.y - 9f, 0.6f));
                    break;
            }

            sequence.AppendCallback(() =>
            {
                obj.gameObject.SetActive(false);
                EventsManager.Publish(new CheckGameCompleteEvent());
            });

            _tile = null;
        }

        private void OnDisable()
        {
            _destroyRockVFX.SetActive(false);
        }
    }
}