using UnityEngine;

namespace BloomLines
{
    public class WithoutRotateDeck : AchivementBase
    {
        //–азложить пась€нс не переворачива€ колоду
        private bool _usedRotate;

        protected override void Start()
        {
            base.Start();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;
            Hub.OnRotateDeck += OnRotateDeck;
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart;
            Hub.OnRotateDeck -= OnRotateDeck;
        }

        private void OnGameStart()
        {            
            _usedRotate = false;
        }

        private void OnRotateDeck()
        {
            _usedRotate = true;
        }

        private void OnGameWin()
        {
            if(!_usedRotate)
                StepAdd();
        }
    }
}
