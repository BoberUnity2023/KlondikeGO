using DG.Tweening;
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

        private void Start()
        {
            StartCoroutine(TryAdd());
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
                    //obj.transform.localScale = Vector3.one * Random.Range(0.8f, 1);
                    Vector3 end = Vector3.Lerp(_finishFrom.position, _finishTo.position, Random.value);
                    float time = Random.Range(_timeFailMin, _timeFailMax);
                    obj.transform.DOMove(end, time).OnComplete(()=> obj.SetActive(false));
                    return;
                }
            }
        }
    }
}
