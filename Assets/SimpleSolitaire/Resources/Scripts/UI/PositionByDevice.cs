
using UnityEngine;
using SimpleSolitaire.Controller;

namespace BloomLines
{
    public class PositionByDevice: MonoBehaviour
    {        
        [SerializeField] private Transform _mobilePosition;

        public void Init(Device device)
        {
            if (device == Device.Mobile)
                transform.position = _mobilePosition.position;
        }
    }
}
