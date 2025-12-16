using SimpleSolitaire.Controller;
using UnityEngine;

namespace BloomLines
{
    public class ButtonSpeed : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameObject[] _points;        

        private void Start()
        {
            int speed = _gameManager.Save.Speed;
            SetSpeed(speed);
        }

        public void OnClick()
        {
            int speed = _gameManager.Save.Speed;
            speed += 1;
            if (speed > 2)
                speed = 0;
            SetSpeed(speed);
        }

        private void SetSpeed(int speed)
        {
            _gameManager.Save.Speed = speed;
            for (int i = 0; i < 3; i++)
            {
                _points[i].SetActive(i <= speed);
            }            
        }
    }
}
