using UnityEngine;
using UnityEngine.UI;

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

        //private GameManager _gameManager;
        private CardShirtManager _cardShirtManager;
        private int _price;

        public GameManager GameManager { get; set; }

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
                    return GameManager.Save.GetCardBacks(_id);
                }

                if (_elementType == VisualiseElementType.Card)
                {
                    return GameManager.Save.GetCards(_id);
                }

                if (_elementType == VisualiseElementType.Background)
                {
                    return GameManager.Save.GetBackgrounds(_id);
                }

                return false;                
            }
        }

        public VisualiseElementType Type => _elementType;

        private void Start()
        {
            _cardShirtManager = FindFirstObjectByType<CardShirtManager>();

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
            if (GameManager == null)
                GameManager = FindFirstObjectByType<GameManager>();

            GameManager.OnClickTryBuyBtn(this);
        }

        public void Buy()
        {
            if (GameManager == null)
                GameManager = FindFirstObjectByType<GameManager>();

            _priceField.SetActive(false);

            if (_elementType == VisualiseElementType.CardBack)
                GameManager.Save.SetCardBacks(_id, true);                          

            if (_elementType == VisualiseElementType.Card)
                GameManager.Save.SetCards(_id, true);                

            if (_elementType == VisualiseElementType.Background)
                GameManager.Save.SetBackgrounds(_id, true);

            AudioController _audioController = FindFirstObjectByType<AudioController>();
            _audioController.Play(AudioController.AudioType.Buy);
        }
    }
}