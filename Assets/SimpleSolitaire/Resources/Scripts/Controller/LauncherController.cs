using System.Collections;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
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
#if !UNITY_WEBGL
            //VibrationAssets.Vibration.Init();
#endif

#if VK || OK
            VK vk = FindAnyObjectByType<VK>();
#endif

//#if OK
//            OK ok = FindAnyObjectByType<OK>();
//#endif

            _progressBarFill.DOFillAmount(0.3f, 0.5f).SetEase(_curve);

            AnalyticsController.SendEvent("game_start");

            yield return new WaitForSeconds(0.5f);

            _progressBarFill.DOFillAmount(1f, 1.0f).SetEase(_curve);

            yield return new WaitForSeconds(1.0f);

//#if VK && !UNITY_EDITOR // VK Storage
//            Debug.Log("VK Storage Loading...");
//            while (!vk.IsSaveLoaded)
//                yield return null;
//            Debug.Log("VK Storage Loaded success");
//#endif

//#if OK && !UNITY_EDITOR
//            Debug.Log("OK Storage Loading...");
//            while (!ok.IsSaveLoaded)
//                yield return null;
//            Debug.Log("OK Storage Loaded success");
//#endif 
            //Debug.Log("Scene1.LoadGame(4)");
            SceneManager.LoadScene("2_KlondikeGO");
        }        
    }    
}