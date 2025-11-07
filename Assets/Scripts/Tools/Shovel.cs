using System.Collections;
using BloomLines.Boards;
using BloomLines.Helpers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;

namespace BloomLines.Tools
{
    public class Shovel : EquipmentTool
    {
        private BoardTile _tile;

        public override void Use(BoardTile tile)
        {
            base.Use(tile);

            if (tile.CanUseTool(_toolType))
            {
                AudioController.Play("use_tool");
                _tile = tile;
            }
        }

        protected override void UseDelay()
        {
            if (_tile == null)
                return;

            _board.StartCoroutine(UseAnimation(_tile));

            _tile = null;
        }

        private IEnumerator UseAnimation(BoardTile tile)
        {
            var wait = new WaitForSeconds(0.2f);

            for (int i = 0; i <= 10; i++)
            {
                var tiles = _board.GetTilesInRadius(tile.TileX, tile.TileY, i);

                Vibration.Vibrate(50);

                foreach (var t in tiles)
                {
                    if (t.HaveObject() && t.Object is Plant)
                    {
                        var tileObject = t.Object;
                        var plant = tileObject as Plant;

                        if (plant.PlantData.PlantType != Assets.PlantType.Flower_Crystal)
                        {
                            if (plant.CanMakeLine())
                            {
                                var cutPlantEvent = new CutPlantEvent(t, plant);
                                EventsManager.Publish(cutPlantEvent);
                            }

                            DOTween.Sequence().Append(tileObject.RectTransform.DOScale(0f, 0.3f).SetEase(Ease.InSine)).AppendCallback(() =>
                            {
                                tileObject.gameObject.SetActive(false);
                            });

                            t.RemoveObject(false);
                        }
                    }
                }

                yield return wait;
            }

            if (!_board.CanMakeAnyMove())
            {
                _board.Evolve(false, true);
            }

            EventsManager.Publish(new CheckGameCompleteEvent());
        }
    }
}