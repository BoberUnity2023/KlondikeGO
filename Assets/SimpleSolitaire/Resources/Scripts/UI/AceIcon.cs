using SimpleSolitaire.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class AceIcon : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private Deck[] _decks;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        private void OnEnable()
        {
            StopAllCoroutines();
            SetSprite();
            StartCoroutine(Interval(1));
        }

        private IEnumerator Interval(float time)
        {
            yield return new WaitForSeconds(time);

            if (IsFree)
            {
                SetSprite();
                StartCoroutine(Interval(1));
            }
        }

        //private void Update()
        //{
        //    if (IsFree)
        //    {
        //        SetSprite();                
        //    }
        //}

        private void SetSprite()
        {
            int index = FreeSuits[MyFreeIndex];
            _image.sprite = _sprites[index];            
        }

        int Type(Deck deck)
        {
            if (deck.CardsArray.Count > 0)
                return deck.CardsArray[0].CardType;

            return -1;
        }
               

        List<int> FreeSuits
        {
            get
            {
                List<int> output = new List<int>();

                List<int> busy = BusySuits;
                for (int i = 0; i < 4; i++)
                {
                    if (!busy.Contains(i))
                        output.Add(i);
                }
                return output;
            }
        }

        List<int> BusySuits
        {
            get
            {
                List<int> output = new List<int>();

                for (int i = 0; i < 4; i++)
                {
                    bool isFree = Type(_decks[i]) == -1;
                    if (!isFree)
                    {
                        int suit = _decks[i].CardsArray[0].CardType;
                        output.Add(suit);
                    }                 
                }
                return output;
            }
        }

        int MyFreeIndex
        {
            get 
            {
                int output = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (_index == i)
                        return output;

                    if (_decks[i].CardsArray.Count == 0)
                        output++;
                }
                return output;
            }
        }

        bool IsFree
        {
            get
            {
                return _decks[_index].CardsArray.Count == 0;
            }
        }                        
    }
}
