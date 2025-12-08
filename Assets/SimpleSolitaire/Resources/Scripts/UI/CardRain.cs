using DG.Tweening;
using SimpleSolitaire.Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace BloomLines
{
    public class CardRain : MonoBehaviour
    {
        [SerializeField] private GameObject[] _items;
        [SerializeField] private float _interval;
        [SerializeField] private Transform _startFrom;
        [SerializeField] private Transform _startTo;
        [SerializeField] private Transform _finishFrom;
        [SerializeField] private Transform _finishTo;
        [SerializeField] private float _timeFailMin;
        [SerializeField] private float _timeFailMax;
        [SerializeField] private bool _isSound;
        [SerializeField] private SimpleSolitaire.Controller.AudioController _audioController;

        private void Start()
        {
            StartCoroutine(TryAdd());
        }

        public void Init(SimpleSolitaire.Controller.AudioController audioController)
        {
            _audioController = audioController;
        }

        private IEnumerator TryAdd()
        {
            yield return new WaitForSeconds(_interval);
            Add();
            StartCoroutine(TryAdd());
        }

        private void Add()
        {
            foreach (GameObject obj in _items)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    obj.transform.position = Vector3.Lerp(_startFrom.position, _startTo.position, Random.value);                    
                    Vector3 end = Vector3.Lerp(_finishFrom.position, _finishTo.position, Random.value);
                    float time = Random.Range(_timeFailMin, _timeFailMax);
                    obj.transform.DOMove(end, time).OnComplete(() => { obj.SetActive(false); OnComplete(); });
                    return;
                }
            }
        }

        private void OnComplete()
        {
            if (_isSound)
                _audioController.Play(SimpleSolitaire.Controller.AudioController.AudioType.Hint);
        }
    }
}
