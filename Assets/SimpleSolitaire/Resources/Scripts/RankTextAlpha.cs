using UnityEngine;
using UnityEngine.UI;

public class RankTextAlpha : MonoBehaviour
{
    [SerializeField] private Text _indicator;    
    
    private void Update()
    {
        bool isLeader = _indicator.text == "1" || _indicator.text == "2" || _indicator.text == "3";
        _indicator.color = isLeader ? Color.white : new Color(1, 1, 1, 0.5f);
    }
}
