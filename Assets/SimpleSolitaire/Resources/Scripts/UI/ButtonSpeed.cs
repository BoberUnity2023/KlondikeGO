using SimpleSolitaire.Controller;
using UnityEngine;

namespace BloomLines
{
    public class ButtonSpeed : MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameObject[] _points;
        private string _key = "Speed";//0, 1, 2

        private void Start()
        {
            int speed = Speed;
            SetSpeed(speed);
        }

        public void OnClick()
        {
            int speed = Speed;
            speed += 1;
            if (speed > 2)
                speed = 0;
            SetSpeed(speed);
        }

        private void SetSpeed(int speed)
        {
            Speed = speed;
            for (int i = 0; i < 3; i++)
            {
                _points[i].SetActive(i <= speed);
            }            
        }

        public int Speed
        {
            get
            {
                return PlayerPrefs.GetInt(_key, 1);
            }

            set
            {
                PlayerPrefs.SetInt(_key, value);
                _gameManager.Speed = value;
            }
        }
    }
}
