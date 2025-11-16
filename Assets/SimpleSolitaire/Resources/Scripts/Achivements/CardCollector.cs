using UnityEngine;

namespace BloomLines
{
    public class CardCollector : AchivementBase
    {
        //Открыть 3 любых колоды

        protected override void Start()
        {
            base.Start();
            Hub.OnBoughtDeck += OnBoughtDeck;
            Progress = Count;
        }

        private void OnDestroy()
        {
            Hub.OnBoughtDeck -= OnBoughtDeck;
        }

        private void OnBoughtDeck()
        {
            StepAdd();
        }

        private int Count
        {
            get
            {
                int output = 0;
                for (int i = 0; i < 13; i++)
                {
                    if (PlayerPrefs.GetInt("Cards" + i.ToString(), 0) == 1)
                        output++;
                }
                Debug.Log("Cards: " + output);
                return output;
            }
        }
    }
}