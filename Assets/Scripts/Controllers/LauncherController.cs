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

            _progressBarFill.DOFillAmount(0.3f, 0.5f).SetEase(_curve);

/*#if !UNITY_EDITOR//TODO: CloudSave
#if OK
            Debug.Log("OKController.SaveLoaded");
            while (!OKController.SaveLoaded)
                yield return null;
#endif

#if VK
            while (!VKController.SaveLoaded)
                yield return null;
#endif
#endif*/
            Debug.Log("Scene1.LoadGame(2)");
            SaveManager.LoadAll(); // Загружаем сохранения
            Debug.Log("Scene1.LoadGame(3)");
            IAPController.LoadPurchases(); // Загружаем покупки
            Debug.Log("Scene1.LoadGame(4)");
            //var gameState = SaveManager.GameState; // Ставим нужную громкость
            //var audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            //audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0f, gameState.MusicVolume));
            //audioMixer.SetFloat("SoundVolume", Mathf.Lerp(-80f, 0f, gameState.SoundVolume));            

            AnalyticsController.SendEvent("game_start");

            yield return new WaitForSeconds(0.5f);

            _progressBarFill.DOFillAmount(1f, 0.6f).SetEase(_curve);

            //SaveManager.LoadAll(); // Загружаем сохранения
            //IAPController.LoadPurchases(); // Загружаем покупки

            yield return new WaitForSeconds(0.6f);
            Debug.Log("Scene1.LoadGame(4)");
            SceneManager.LoadScene("2_KlondikeGO");
        }        
    }    
}