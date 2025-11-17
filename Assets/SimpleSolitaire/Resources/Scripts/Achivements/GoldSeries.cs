using UnityEngine;

namespace BloomLines
{
    public class GoldSeries : AchivementBase
    {
        //Выиграть 5 игр подряд
        private const string _key = "AchGoldSeriesRound";
        protected override void Start()
        {
            base.Start();
            Hub.OnGameWin += OnGameWin;
            Hub.OnGameStart += OnGameStart;            
        }

        private void OnDestroy()
        {
            Hub.OnGameWin -= OnGameWin;
            Hub.OnGameStart -= OnGameStart;            
        }

        private void OnGameStart()
        {
            if (IsComplete)
                return;

            if (Round > Progress)
            { 
                Progress = 0;
                Round = 0;
            }

            Round++;
        }

        private void OnGameWin()
        { 
            StepAdd();            
        }   
        
        private int Round//TODO: SaveToCloud
        {
            get 
            { 
                return PlayerPrefs.GetInt(_key, 0); 
            }
            set 
            { 
                PlayerPrefs.SetInt(_key, value);
                PlayerPrefs.Save();
            }
        }
    }
}
