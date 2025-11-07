using System;
using System.Collections;
using System.Collections.Generic;
using BloomLines.Controllers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.Tutorial
{
    public class TutorialHighlightObjectData
    {
        public bool AlreadyHaveImage;
        public bool AlreadyHaveButton;
        public bool AlreadyHaveCanvas;
        public bool AlreadyHaveRaycast;
        public bool MainRaycastTarget;
        public int MainSortingOrder;
        public bool MainOverrideSorting;
    }

    public abstract class TutorialBase : MonoBehaviour
    {
        [SerializeField] protected string _id;
        [SerializeField] private CanvasGroup _tutorialHand;
        [SerializeField] private ParticleSystem _handVfx;

        public string Id => _id;

        private CanvasGroup _canvasGroup;
        private Action<TutorialBase> _onComplete;

        private Dictionary<int, TutorialHighlightObjectData> _highlightObjects;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _highlightObjects = new Dictionary<int, TutorialHighlightObjectData>();
        }

        protected void HighlightObject(GameObject obj)
        {
            var id = obj.GetInstanceID();
            if (!_highlightObjects.ContainsKey(id))
            {
                var data = new TutorialHighlightObjectData();

                var button = obj.GetComponent<Button>();
                var image = obj.GetComponent<Image>();
                var canvas = obj.GetComponent<Canvas>();
                var raycast = obj.GetComponent<GraphicRaycaster>();

                data.AlreadyHaveImage = image != null;
                data.AlreadyHaveButton = button != null;
                data.AlreadyHaveCanvas = canvas != null;
                data.AlreadyHaveRaycast = raycast != null;

                _highlightObjects.Add(id, data);

                if (!data.AlreadyHaveButton)
                {
                    button = obj.AddComponent<Button>();
                    button.transition = Selectable.Transition.None;
                }

                if (!data.AlreadyHaveImage)
                {
                    image = obj.AddComponent<Image>();
                    image.color = Color.clear;
                }

                if (!data.AlreadyHaveCanvas)
                    canvas = obj.AddComponent<Canvas>();

                if (!data.AlreadyHaveRaycast)
                    raycast = obj.AddComponent<GraphicRaycaster>();

                data.MainRaycastTarget = image.raycastTarget;
                data.MainSortingOrder = canvas.sortingOrder;
                data.MainOverrideSorting = canvas.overrideSorting;

                image.raycastTarget = true;
                canvas.overrideSorting = true;
                canvas.sortingOrder = 301;
            }
        }

        protected void RemoveHighlight(GameObject obj)
        {
            var id = obj.GetInstanceID();
            if (_highlightObjects.ContainsKey(id))
            {
                var data = _highlightObjects[id];

                var button = obj.GetComponent<Button>();
                var image = obj.GetComponent<Image>();
                var canvas = obj.GetComponent<Canvas>();
                var raycast = obj.GetComponent<GraphicRaycaster>();

                image.raycastTarget = data.MainRaycastTarget;
                canvas.sortingOrder = data.MainSortingOrder;
                canvas.overrideSorting = data.MainOverrideSorting;

                if (!data.AlreadyHaveButton)
                    Destroy(button);

                if (!data.AlreadyHaveImage)
                    Destroy(image);

                if (!data.AlreadyHaveRaycast)
                    Destroy(raycast);

                if (!data.AlreadyHaveCanvas)
                    Destroy(canvas);

                _highlightObjects.Remove(id);
            }
        }

        protected IEnumerator WaitClick(GameObject obj)
        {
            gameObject.GetInstanceID();

            HighlightObject(obj);

            var button = obj.GetComponent<Button>();
            bool clicked = false;

            void OnClick() => clicked = true;

            button.onClick.AddListener(OnClick);

            while (!clicked)
                yield return null;

            button.onClick.RemoveListener(OnClick);

            RemoveHighlight(obj);
        }

        protected void ShowHand(Vector3 position)
        {
            _handVfx.Play();
            _tutorialHand.DOFade(1f, 0.2f);
            _tutorialHand.transform.position = position;
        }

        protected void HideHand()
        {
            _handVfx.Stop();
            _tutorialHand.DOFade(0f, 0.2f);
        }

        protected void CompleteTutorial()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _onComplete?.Invoke(this);
            _onComplete = null;

            HideHand();
        }

        public virtual void StartTutorial(Action<TutorialBase> onComplete)
        {
            _tutorialHand.alpha = 0f;
            _handVfx.Stop();
            _handVfx.Clear();

            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _onComplete = onComplete;

            HideHand();
        }

        protected virtual void OnEnable()
        {
            TutorialController.RegisterTutorial(this);
        }

        protected virtual void OnDisable()
        {
            TutorialController.RemoveTutorial(this);
        }
    }
}