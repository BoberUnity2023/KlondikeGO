using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BloomLines.Controllers
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private Image _progressBarFill;
        [SerializeField] private AnimationCurve _curve;

        private void Start()
        {
            _progressBarFill.fillAmount = 0f;
            AnalyticsController.SendEvent("game_load_start");
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            Debug.Log("Scene1.LoadGame(1)");

            _progressBarFill.DOFillAmount(0.3f, 0.5f).SetEase(_curve);

            AnalyticsController.SendEvent("game_start");

            yield return new WaitForSeconds(0.5f);

            _progressBarFill.DOFillAmount(1f, 1.0f).SetEase(_curve);

            yield return new WaitForSeconds(1.0f);
            //Debug.Log("Scene1.LoadGame(4)");
            SceneManager.LoadScene("2_KlondikeGO");
        }        
    }    
}