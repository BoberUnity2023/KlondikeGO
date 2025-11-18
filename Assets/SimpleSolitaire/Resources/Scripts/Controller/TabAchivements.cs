using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class TabAchivements : MonoBehaviour
    {
        [SerializeField]
        private AchivementsController _achivementsController;
        [SerializeField] private Image _fill;
        [SerializeField] private Text _progressIndicator;

        private void OnEnable()
        {
            int count = _achivementsController.CompletedAchivements;
            _progressIndicator.text = count.ToString() + "/12";
            _fill.fillAmount = (float)count / 12;

        }
    }
}
