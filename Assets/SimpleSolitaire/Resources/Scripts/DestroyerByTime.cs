using UnityEngine;

public class DestroyerByTime : MonoBehaviour
{
    [SerializeField] private float _time;

    void Start()
    {
        Destroy(gameObject, _time);
    }    
}
