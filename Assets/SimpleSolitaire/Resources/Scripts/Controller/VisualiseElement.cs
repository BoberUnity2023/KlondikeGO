using UnityEngine;
using UnityEngine.UI;
//using YG;

namespace SimpleSolitaire.Controller
{
    public enum VisualiseElementType
    {
        Background,
        CardBack,
        Card
    }

    public class VisualiseElement : MonoBehaviour
	{
        [SerializeField] private VisualiseElementType _elementType;
        [SerializeField] private int _id;
        public Image VisualImage;
		public Image CheckMark;
		public Animator Anim;
		public Button Btn;
        [SerializeField] private GameObject _priceField;
        [SerializeField] private Text _priceIndicator;
        [HideInInspector] public string ElementName;

        private GameManager _gameManager;
        private CardShirtManager _cardShirtManager;
        private int _price;

        public int Price
        {
            get
            {
                return _price;

            }
            set
            {
                _price = value;
                _priceField.SetActive(value > 0 && !HasBought);
                _priceIndicator.text = value.ToString();
            }
        }

        public bool HasBought
        {
            get
            {
                if (_elementType == VisualiseElementType.CardBack)
                {                    
                    return HasCardBack(_id);
                }

                if (_elementType == VisualiseElementType.Card)
                {
                    return HasCards(_id);
                }

                if (_elementType == VisualiseElementType.Background)
                {
                    return PlayerPrefs.GetInt("Background" + _id.ToString()) == 1;

                    //if (_id >= YandexGame.savesData.Backgrounds.Length)
                    //    return false;

                    //return YandexGame.savesData.Backgrounds[_id];
                }

                return false;
                //return PlayerPrefs.GetInt(gameObject.name) > 0;
            }
        }

        public VisualiseElementType Type => _elementType;

        private bool HasCardBack(int id)
        {
            return PlayerPrefs.GetInt("CardBack" + _id.ToString()) == 1;

            //if (id < YandexGame.savesData.CardBacks.Length)
            //    return YandexGame.savesData.CardBacks[_id] || YandexGame.savesData.CardBacks2[_id]; 

            //return YandexGame.savesData.CardBacks2[_id];
        }

        private bool HasCards(int id)
        {
            return PlayerPrefs.GetInt("Cards" + _id.ToString()) == 1;

            //if (id < YandexGame.savesData.Cards.Length)
            //    return YandexGame.savesData.Cards[_id] || YandexGame.savesData.Cards2[_id];

            //return YandexGame.savesData.Cards2[_id];
        }        

        private void Start()
        {
            _cardShirtManager = FindObjectOfType<CardShirtManager>();

            switch (_elementType)
            {
                case VisualiseElementType.CardBack:
                    {
                        Price = _cardShirtManager.CardBackVisual.Prices[_id];
                        break;
                    }
                case VisualiseElementType.Background:
                    {
                        Price = _cardShirtManager.BackgroundVisual.Prices[_id];
                        break;
                    }
                case VisualiseElementType.Card:
                    {
                        Price = _cardShirtManager.CardFrontVisual.Prices[_id];
                        break;
                    }
            }
        }

        public void ActivateCheckmark()
		{
			CheckMark.enabled = true;
		}

		public void DeactivateCheckmark()
		{
			CheckMark.enabled = false;
		}

        public void PressTryBuy()
        {
            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();

            _gameManager.OnClickTryBuyBtn(this);
        }

        public void Buy()
        {
            if (_gameManager == null)
                _gameManager = FindObjectOfType<GameManager>();

            _priceField.SetActive(false);
            //PlayerPrefs.SetInt(gameObject.name, 1);

            if (_elementType == VisualiseElementType.CardBack)
                //YandexGame.savesData.CardBacks2[_id] = true;
                PlayerPrefs.SetInt("CardBack" + _id.ToString(), 1);            

            if (_elementType == VisualiseElementType.Card)
                //YandexGame.savesData.Cards2[_id] = true;
                PlayerPrefs.SetInt("Cards" + _id.ToString(), 1);

            if (_elementType == VisualiseElementType.Background)
                //YandexGame.savesData.Backgrounds[_id] = true;
                PlayerPrefs.SetInt("Background" + _id.ToString(), 1);

            PlayerPrefs.Save();
            //YandexGame.SaveProgress();

            AudioController _audioController = FindObjectOfType<AudioController>();
            _audioController.Play(AudioController.AudioType.Buy);
        }
    }
}