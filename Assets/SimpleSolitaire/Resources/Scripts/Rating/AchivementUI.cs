using UnityEngine;
using UnityEngine.UI;

public class AchivementUI : MonoBehaviour
{
    [SerializeField] private Image _icon;

    public void ShowComplete()
    {
        _icon.color = new Color(1, 1, 1, 1);
    }

    public void ShowNoComplete()
    {        
        _icon.color = new Color(1, 1, 1, 0.4f);
    }
}
