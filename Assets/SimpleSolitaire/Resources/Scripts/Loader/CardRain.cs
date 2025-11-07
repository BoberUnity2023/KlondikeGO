using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CardRain : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private Canvas _canvas;
    void Start()
    {
        StartCoroutine(CreateCard(1));
    }

    private IEnumerator CreateCard(float time)
    {
        yield return new WaitForSeconds(time);
        int id = Random.Range(0, _prefabs.Length);
        GameObject prefab = _prefabs[id];

        Vector3 pos = Vector3.right * Random.Range(-1000, 1000) + Vector3.up * 1000;
        GameObject item = Instantiate(prefab, pos, Quaternion.identity, _canvas.transform);
        Vector3 finish = new Vector3(pos.x, -2000, 0);
        item.transform.DOMove(finish, 3);
        Destroy(item, 3);
        StartCoroutine(CreateCard(1));
    }
}
