using SimpleSolitaire.Controller;
using System.Collections;
using UnityEngine;

public class ScreenDebug : MonoBehaviour
{
    [SerializeField] KlondikeCardLogic _klondikeCardLogic;
    void Start()
    {
        //StartCoroutine(ShowInfo());
    }

    
    private IEnumerator ShowInfo()
    {
        yield return new WaitForSeconds(12);
        float width = Screen.width;
        float height = Screen.height;
        Debug.LogWarning("Screen size: " + width + "x" + height);
        Debug.LogWarning("Deck size: " + (int)_klondikeCardLogic.DeckHeight + "x" + (int)_klondikeCardLogic.DeckWidth);    
        
        StartCoroutine(ShowInfo());
    }
}
