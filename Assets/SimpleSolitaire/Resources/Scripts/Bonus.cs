using UnityEngine;
using UnityEngine.UI;

public class Bonus : MonoBehaviour
{
    [SerializeField] private Text _goldIndicator;
    [SerializeField] private Text _xIndicator;

    public void SetIndicatorors(int level)
    {        
        _goldIndicator.text = (100 * (level + 1)).ToString();
        _xIndicator.text = "x" + (level + 1).ToString();
    }    
}
