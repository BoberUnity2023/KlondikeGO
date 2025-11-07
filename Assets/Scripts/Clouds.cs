using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class Clouds : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Material _material;

        private void Awake()
        {
            var img = GetComponent<Image>();
            _material = img.material;
        }

        private void Update()
        {
            _material.mainTextureOffset += Vector2.left * _speed * Time.deltaTime;
        }
    }
}