using System;
using System.Collections;
using System.Collections.Generic;
using BloomLines.Assets;
using BloomLines.Boards;
using BloomLines.Helpers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BloomLines.Tools
{
    public class Secateurs : EquipmentTool
    {
        private BoardTile _tile;

        public override void Use(BoardTile tile)
        {
            if (!IsActive)
                return;

            if (tile.CanUseTool(_toolType))
            {
                _shakeTween?.Complete(true);
                _shakeTween = null;

                IsActive = false;

                Cursor.Cursor.ReleaseTool();
                EventsManager.Publish(new ResetToolEvent(_toolType));

                StartCoroutine(UseAnimation());
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

        protected override void UseDelay()
        {
        }

        private IEnumerator UseAnimation()
        {
            var targetTiles = new List<BoardTile>();

            foreach (var tile in _board.Tiles)
            {
                if(tile.HaveObject() && tile.Object is Plant && (tile.Object as Plant).PlantData.PlantType == PlantType.Weed)
                {
                    targetTiles.Add(tile);
                }
            }

            if (targetTiles.Count <= 0)
                yield break;

            var wait = new WaitForSeconds(0.30f);

            int animationCount = Mathf.Min(3, targetTiles.Count);
            int part = Mathf.Max(Mathf.FloorToInt((float)targetTiles.Count / animationCount), 1);

            for (int i = 0; i < animationCount; i++)
            {
                if (targetTiles.Count <= 0)
                    break;

                int index = Random.Range(0, targetTiles.Count);
                var tile = targetTiles[index];

                _icon.localScale = Vector3.zero;
                _rectTransform.position = tile.RectTransform.position;

                _anim.SetTrigger("Show");

                yield return wait;

                _anim.SetTrigger("Use");

                yield return wait;

                DestroyPlant(tile);
                targetTiles.RemoveAt(index);

                AudioController.Play("use_secateurs");
                Vibration.Vibrate(50);

                int destroyCount = part - 1;
                if (i >= animationCount - 1|| destroyCount > targetTiles.Count)
                    destroyCount = targetTiles.Count;

                for (int b = 0; b < destroyCount; b++)
                {
                    index = Random.Range(0, targetTiles.Count);
                    tile = targetTiles[index];

                    DestroyPlant(tile);
                    targetTiles.RemoveAt(index);
                }

                yield return wait;
            }

            yield return wait;

            _anim.SetTrigger("Hide");

            if (!_board.CanMakeAnyMove())
            {
                _board.Evolve(false, true);
            }

            EventsManager.Publish(new CheckGameCompleteEvent());
        }

        private void DestroyPlant(BoardTile tile)
        {
            var tileObject = tile.Object;
            var plant = tileObject as Plant;

            if (plant.CanMakeLine())
            {
                var cutPlantEvent = new CutPlantEvent(tile, plant);
                EventsManager.Publish(cutPlantEvent);
            }

            DOTween.Sequence().Append(tileObject.RectTransform.DOScale(0f, 0.3f).SetEase(Ease.InSine)).AppendCallback(() =>
            {
                tileObject.gameObject.SetActive(false);
            });

            tile.RemoveObject(false);
        }
    }
}