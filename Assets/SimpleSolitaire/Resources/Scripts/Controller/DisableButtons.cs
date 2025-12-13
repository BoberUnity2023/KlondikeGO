using UnityEngine;
using UnityEngine.UI;

namespace BloomLines
{
    public class DisableButtons : MonoBehaviour
    {
        [SerializeField] private Button[] _buttons;

        public void Activate()
        {
            foreach (Button button in _buttons)
            {
                button.interactable = true;
            }
        }

        public void Deactivate()
        {
            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
        }
    }
}
