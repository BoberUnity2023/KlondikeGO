using System.Collections;
using BloomLines.Controllers;
using BloomLines.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#if GAME_PUSH
using GamePush;
#endif

#if Yandex
using YG;
#endif

namespace BloomLines.UI
{
    public class UIRateGamePanel : UIPanelBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _rateButton;
        [SerializeField] private Transform[] _stars;
        [SerializeField] private Transform[] _starsFill;

        protected override void Awake()
        {
            base.Awake();

            _closeButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_rate_game_close_button");
                Close();
            });

            _backgroundButton.onClick.AddListener(() =>
            {
                AnalyticsController.SendEvent("click_rate_game_close_button");
                Close();
            });

            _rateButton.onClick.AddListener(() =>
            {
                AudioController.Play("click_button");
                AnalyticsController.SendEvent("click_rate_game_write_button");

#if Yandex
                YG2.ReviewShow();
#endif

#if VK
                Application.OpenURL("https://vk.com/dakagames");
#endif

#if OK
                Application.OpenURL("https://yandex.com/games/developer/71014");
#endif

#if RuStore 
                Application.OpenURL("https://www.rustore.ru/catalog/app/com.DakaGames.BloomLines");
#endif

                Close();
            });
        }

        private IEnumerator StarsAnimation()
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].localScale = Vector3.zero;
                _starsFill[i].localScale = Vector3.zero;
            }

            yield return new WaitForSeconds(0.5f);

            var interval = new WaitForSeconds(0.2f);

            _stars[2].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            _starsFill[2].DOScale(1f, 0.4f).SetEase(Ease.InOutSine);

            yield return interval;

            _stars[1].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            _starsFill[1].DOScale(1f, 0.3f).SetEase(Ease.InOutSine);

            _stars[3].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            _starsFill[3].DOScale(1f, 0.3f).SetEase(Ease.InOutSine);

            yield return interval;

            _stars[0].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            _starsFill[0].DOScale(1f, 0.3f).SetEase(Ease.InOutSine);

            _stars[4].DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            _starsFill[4].DOScale(1f, 0.3f).SetEase(Ease.InOutSine);
        }

        protected override void Open()
        {
            base.Open();

            StartCoroutine(StarsAnimation());
        }
    }
}