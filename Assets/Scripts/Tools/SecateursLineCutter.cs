using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BloomLines.Boards;
using UnityEngine;
using DG.Tweening;
using BloomLines.Managers;
using BloomLines.Assets;

namespace BloomLines.Tools
{
    public class CutPlantEvent
    {
        private BoardTile _tile;
        private Plant _plant;

        public BoardTile Tile => _tile;
        public Plant Plant => _plant;

        public CutPlantEvent(BoardTile tile, Plant plant)
        {
            _tile = tile;
            _plant = plant;
        }
    }

    public class SecateursLineCutter : MonoBehaviour
    {
        private BoardTile _targetBoardTile;
        private RectTransform _rectTransform;
        private Animator _anim;
        private Queue<HashSet<BoardTile>> _lines = new Queue<HashSet<BoardTile>>();

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_targetBoardTile != null)
                _rectTransform.sizeDelta = _targetBoardTile.RectTransform.sizeDelta;
        }

        public IEnumerator CutTheLine(HashSet<BoardTile> line)
        {
            if (line.Count <= 2)
                yield break;

            _lines.Enqueue(line);

            if (_lines.Count > 1)
                yield break;

            IsPlaying = true;

            yield return null;

            gameObject.SetActive(true);

            while (_lines.Count > 0)
            {
                _anim.SetTrigger("Show");

                var activeLine = _lines.Peek();
                _targetBoardTile = activeLine.FirstOrDefault();

                yield return StartCoroutine(CutTheLineAnimation(activeLine));

                _lines.Dequeue();
            }

            gameObject.SetActive(false);
            IsPlaying = false;
        }

        private IEnumerator CutTheLineAnimation(HashSet<BoardTile> line)
        {
            var secondTile = line.ElementAt(1);
            var rotation = GetRotationBetweenTiles(_targetBoardTile, secondTile);
            var distanceBetweenTiles = Vector3.Distance(_targetBoardTile.RectTransform.position, secondTile.RectTransform.position);

            _rectTransform.position = _targetBoardTile.RectTransform.position - ((secondTile.RectTransform.position - _targetBoardTile.RectTransform.position) * 0.8f);
            _rectTransform.rotation = rotation;

            yield return new WaitForSeconds(0.8f);

            var moveTime = 0.17f;
            var wait = new WaitForSeconds(moveTime);

            int count = 0;
            bool isWeed = line.FirstOrDefault(e => e.HaveObject() && (e.Object as Plant).PlantData.PlantType == PlantType.Weed);

            BoardTile prevTile = _targetBoardTile;
            foreach (var tile in line)
            {
                if (prevTile != _targetBoardTile)
                {
                    rotation = GetRotationBetweenTiles(prevTile, tile);

                    var distance = Vector3.Distance(prevTile.RectTransform.position, tile.RectTransform.position);
                    moveTime *= (distance / distanceBetweenTiles);
                }

                DOTween.Sequence()
                    .Append(_rectTransform.DOMove(tile.RectTransform.position, moveTime).SetEase(Ease.Linear))
                    .Join(_rectTransform.DORotateQuaternion(rotation, moveTime * 0.8f).SetEase(Ease.InOutSine));

                prevTile = tile;

                if (count == 0 || count >= line.Count - 1)
                    AudioController.Play("use_secateurs");

                yield return wait;

                if(count == 0)
                    AudioController.Play("connect_line");

                if (count == 3 && !isWeed)
                    AudioController.Play("collect_cut_plants");

                if (tile.HaveObject() && tile.Object.Data.ObjectType == BoardObjectType.Plant)
                {
                    var tileObject = tile.Object;
                    var cutPlantEvent = new CutPlantEvent(tile, tileObject as Plant);
                    EventsManager.Publish(cutPlantEvent);

                    DOTween.Sequence().Append(tileObject.RectTransform.DOScale(0f, 0.3f).SetEase(Ease.InSine)).AppendCallback(() =>
                    {
                        tileObject.gameObject.SetActive(false);
                    });

                    tile.RemoveObject(false);
                }

                count++;
            }

            _anim.SetTrigger("Stop");

            yield return new WaitForSeconds(0.35f);
        }

        private Quaternion GetRotationBetweenTiles(BoardTile from, BoardTile to)
        {
            Vector2 dir = to.RectTransform.position - from.RectTransform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle -= 180;

            return Quaternion.Euler(0, 0, angle);
        }
    }
}