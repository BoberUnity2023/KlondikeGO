using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSolitaire.Controller;
using Coffee.UIEffects;

public class CardDragEffect : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Animator _shineAnimator;    
    [SerializeField] private Animator _iconAnimator;
    [SerializeField] private UIShiny _uIShiny;
    [SerializeField] private Animator _shadowAnimator;
    [SerializeField] private RectTransform _shadowRectTransform;
    private List<Card> _cardsToTop;
    private AudioController _audioController;
    private bool _isEffectStarted;

    public void On(List<Card> cardsToTop = null)
    {
        _isEffectStarted = true;
        StopAllCoroutines();
        _cardsToTop = cardsToTop;
        _shadowAnimator.ResetTrigger("Off");
        _shadowAnimator.SetTrigger("On");        
        int space = _gameManager.Game == Game.Klondike ? 60 : 48;
        _shadowRectTransform.sizeDelta = new Vector2(154, 210 + (cardsToTop.Count - 1) * space);

        foreach (var item in cardsToTop)
        {            
            item.GetComponent<CardDragEffect>().SetCardLightOn();
        }

        if (_audioController == null)
            _audioController = FindAnyObjectByType<AudioController>();

        _audioController.Play(AudioController.AudioType.CardTake);
    }

    public void Off()
    {
        if (!_isEffectStarted)
            return;

        _isEffectStarted = false;

        _shadowAnimator.ResetTrigger("On");
        _shadowAnimator.SetTrigger("Off");
        if (_cardsToTop != null)
        {
            foreach (var item in _cardsToTop)
            {
                item.GetComponent<CardDragEffect>().SetCardLightOff();
            }
        }
        else
            Debug.LogWarning("_cardsToTop == null");

        StartCoroutine(AfterOff());

        _uIShiny.Play();        

        _audioController.Play(AudioController.AudioType.CardPut);
    }

    private IEnumerator AfterOff()
    {
        yield return new WaitForSeconds(0.15f);
        _shadowRectTransform.sizeDelta = new Vector2(154, 210);        
    }

    private void SetCardLightOn()
    {
        _shineAnimator.ResetTrigger("Off");
        _shineAnimator.SetTrigger("On");

        _iconAnimator.ResetTrigger("Off");
        _iconAnimator.SetTrigger("On");        
    }

    private void SetCardLightOff()
    {
        _shineAnimator.ResetTrigger("On");
        _shineAnimator.SetTrigger("Off");

        _iconAnimator.ResetTrigger("On");
        _iconAnimator.SetTrigger("Off");
    }

    public void Reset()
    {
        _shadowRectTransform.sizeDelta = new Vector2(154, 210);
    }
}
