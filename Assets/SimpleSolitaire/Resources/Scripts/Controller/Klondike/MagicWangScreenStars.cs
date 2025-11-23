using System.Collections;
using UnityEngine;

namespace BloomLines
{
    public class MagicWangScreenStars : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;  

        public void EffectStart(Transform parent)
        {
            gameObject.SetActive(true);
            transform.SetParent(parent, false);
            ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
            emissionModule.enabled = true;
            StopAllCoroutines();
        }

        public void EffectStop()
        {
            ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
            emissionModule.enabled = false;
            StartCoroutine(Off(1));
        }

        private IEnumerator Off(float time)
        {
            yield return new WaitForSeconds(time);
            gameObject.SetActive(false);
        }
    }
}
