using SimpleSolitaire.Controller;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//using YG;

namespace SimpleSolitaire.Screen
{
    //1920x1080 - 0,5625 - 1080
    //0,6 - 1080
    //1 - 1800
    public enum ScreenOrientation
    {
        Horizontal, 
        Vertical
    }    

    [Serializable] public class ScreenOrientationPosition
    {
        public Transform Transform;    
        public Transform PositionVerical;

        public Vector3 PositionHorizontal { get; set; }
    }

    [Serializable]
    public class ScreenOrientationScale
    {
        public Transform Transform;
        public float ScaleVerical;

        public Vector3 ScaleHorizontal { get; set; }
    }

    public class ScreenController: MonoBehaviour
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private HintManager _hintManager;
        //[SerializeField] private YandexGame _yg;
        [SerializeField] private CanvasScaler _canvasScaler;        
        [SerializeField] private ScreenOrientation _orientation;
        [SerializeField] private float _changeAspectRatio;
        [SerializeField] private float _startScaleHorizontal;
        [SerializeField] private float _a;
        [SerializeField] private ScreenOrientationPosition[] _screenOrientationPositions;        
        [SerializeField] private Deck[] _decks;
        [SerializeField] private RectTransform _alignByTop;
        [SerializeField] private ScreenOrientationScale[] _screenOrientationScales;
        [SerializeField] private ScreenTopPanelController _screenTopPanelController;
        private int _fapiWindowHeight;

        public ScreenOrientation Orientation => _orientation;

        public CanvasScaler CanvasScaler => _canvasScaler;

        public bool Active { get; set; }

        public bool NeedUpdate { get; set; }

        void Start()
        {
            Active = true;
            _orientation = ScreenOrientation.Horizontal;
            foreach (var item in _screenOrientationPositions)
            {
                item.PositionHorizontal = item.Transform.localPosition;                
            }

            foreach (var item in _screenOrientationScales)
            {
                item.ScaleHorizontal = item.Transform.localScale;
            }
            SetScreen();
            StartCoroutine(UpdateCheck());
        }

        public void SetFapiWindowHeight(int height)
        {
            float w = UnityEngine.Screen.width;
            float h = UnityEngine.Screen.height;
            Debug.Log("Unity Screen size: " + w + " x " + h + "; FAPI h: " + height);
            _fapiWindowHeight = height;
        }
    
        private IEnumerator UpdateCheck()
        {
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(UpdateCheck());
            SetScreen();
        }

        private void SetScreen()
        {
            if (!Active)
                return;

            float width = UnityEngine.Screen.width; 
            float height = UnityEngine.Screen.height;
            
            //Debug.LogWarning("Deck size: " + (int)_klondikeCardLogic.DeckHeight + "x" + (int)_klondikeCardLogic.DeckWidth);

            float aspectRatio = height / width;
            _a = aspectRatio;
            _screenTopPanelController.SetAspectRatio(aspectRatio);

            //Debug.LogWarning("Screen size: " + width + "x" + height + " aspectRatio: " + aspectRatio.ToString("f2"));

            if (aspectRatio > _changeAspectRatio && ( _orientation == ScreenOrientation.Horizontal || NeedUpdate))
            {
                SetVertical();
                NeedUpdate = false;
            }

            if (aspectRatio < _changeAspectRatio && (_orientation == ScreenOrientation.Vertical || NeedUpdate))
            {
                SetHorizontal();
                NeedUpdate = false;
            }

            SetCanvasSize();

            CanvasScaler canvas = null;

            if (_gameManager.Game == Game.Klondike)
                canvas = _canvasScaler;

            float y = 200 - 1080 + canvas.referenceResolution.y;
            _alignByTop.offsetMax = new Vector2(_alignByTop.offsetMax.x, y); 
        }

        private void SetCanvasSize()
        {
            float canvasHeight = 1080;

            float width = UnityEngine.Screen.width;
            float height = UnityEngine.Screen.height;
            float aspectRatio = height / width;

            if (_orientation == ScreenOrientation.Horizontal)
            {
                if (aspectRatio < _startScaleHorizontal)
                { 
                    _canvasScaler.referenceResolution = new Vector2(1920, 1080);
                }

                if (aspectRatio > _startScaleHorizontal && aspectRatio < _changeAspectRatio)
                {
                    canvasHeight = 1080 + (aspectRatio - _startScaleHorizontal) * 1500;
                    _canvasScaler.referenceResolution = new Vector2(_canvasScaler.referenceResolution.x, canvasHeight);                    
                }
            }


            if (_orientation == ScreenOrientation.Vertical)
            {
                float add = 0;
                if (aspectRatio < 1.5f)
                    add = (1.5f - aspectRatio) * 2000;

                canvasHeight = aspectRatio * 1080 + (int)add;
                _canvasScaler.referenceResolution = new Vector2(_canvasScaler.referenceResolution.x, canvasHeight);
            }            
        }

        private void SetVertical()
        {
            Debug.Log("SetVertical");

            foreach (var item in _screenOrientationPositions)
            {                
                item.Transform.localPosition = item.PositionVerical.localPosition;
            }

            foreach (var item in _screenOrientationScales)
            {
                if (_orientation == ScreenOrientation.Horizontal)
                item.ScaleHorizontal = item.Transform.localScale;
                item.Transform.localScale = Vector3.one * item.ScaleVerical;
            }

            StartCoroutine(SetDecks());
            _orientation = ScreenOrientation.Vertical;
            SetCanvasSize();
            _hintManager.GenerateHints();            
        }

        private void SetHorizontal()
        {
            Debug.Log("SetHorizontal");
            _orientation = ScreenOrientation.Horizontal;
            SetCanvasSize();

            foreach (var item in _screenOrientationPositions)
            {            
                item.Transform.localPosition = item.PositionHorizontal;
            }

            foreach (var item in _screenOrientationScales)
            {                
                item.Transform.localScale = item.ScaleHorizontal;
            }

            StartCoroutine(SetDecks());

            _hintManager.GenerateHints();            
        }

        private IEnumerator SetDecks()
        {
            yield return new WaitForEndOfFrame();
            foreach (var deck in _decks)
            {
                deck.UpdateCardsPosition(false);
            }
        }
    }
}
