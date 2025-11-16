using UnityEngine;

namespace BloomLines
{
    public class Atmosphere : AchivementBase
    {
        //Открыть 3 любых фона

        protected override void Start()
        {
            base.Start();
            Hub.OnBoughtBackground += OnBoughtBackground;
            Progress = Count;
        }

        private void OnDestroy()
        {
            Hub.OnBoughtBackground -= OnBoughtBackground;            
        }

        private void OnBoughtBackground()
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
                    if (PlayerPrefs.GetInt("Background" + i.ToString(), 0) == 1)
                        output++;
                }
                Debug.Log("CountBacks: " + output);
                return output;
            }
        }
    }
}
