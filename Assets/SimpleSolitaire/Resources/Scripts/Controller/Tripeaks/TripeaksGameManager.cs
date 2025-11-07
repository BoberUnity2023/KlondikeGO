using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class TripeaksGameManager : GameManager
    {
        [Header("Tripeaks Layers:")]
        [SerializeField]
        private GameObject _layoutsSettings;
        [SerializeField]
        private RectTransform _layoutsContent;
        [SerializeField]
        private TripeaksLayoutContainer _layoutsContainer;

        [SerializeField]
        private VisualiseElement _layout;
        private List<VisualiseElement> _layouts = new List<VisualiseElement>();
        private TripeaksCardLogic _tripeaksCardLogic => _cardLogic as TripeaksCardLogic;

        protected override void InitializeGame()
        {
            base.InitializeGame();

            _layoutsContainer.GetLayoutsSettings();
            GenerateLayoutsPreviews();
        }

        private void GenerateLayoutsPreviews()
        {
            for (int i = 0; i < _layoutsContainer.Layouts.Count; i++)
            {
                TripeaksLayoutData layoutInfo = _layoutsContainer.Layouts[i];
                VisualiseElement layoutVisual = Instantiate(_layout, _layoutsContent);
                if (_layoutsContainer.ActiveLayouts.Contains(layoutInfo.LayoutId))
                    layoutVisual.ActivateCheckmark();
                else
                    layoutVisual.DeactivateCheckmark();

                layoutVisual.VisualImage.sprite = layoutInfo.Preview;

                layoutVisual.Btn.onClick.AddListener(() => OnLayoutClicked(layoutVisual, layoutInfo));
                layoutVisual.gameObject.SetActive(true);

            }
        }

        private void OnLayoutClicked(VisualiseElement layoutVisual, TripeaksLayoutData layoutInfo)
        {
            var alreadyActive = _layoutsContainer.IsActiveLayout(layoutInfo.LayoutId);
            if (alreadyActive)
            {
                if (_layoutsContainer.HasOneOrLessLayout())
                {
                    return;
                }

                layoutVisual.DeactivateCheckmark();
                _layoutsContainer.RemoveLayout(layoutInfo.LayoutId);
            }
            else
            {
                layoutVisual.ActivateCheckmark();
                _layoutsContainer.AddLayout(layoutInfo.LayoutId);
            }

            _layoutsContainer.SaveLayouts();
        }

        protected override void InitCardLogic()
        {
            _tripeaksCardLogic.InitCurrentLayout();
        }

        public void OnClickLayoutSettingBtn()
        {
            StartCoroutine(InvokeAction(delegate { OnClickModalClose(); Invoke(nameof(OnLayoutSettingsAppearing), _windowAnimationTime); }, 0f));
        }

        /*protected override void OnModalLayerDisappeared()
        {
            _newGameLayer.SetActive(false);
            _cardLayer.SetActive(!_newGameLayer.activeInHierarchy && !_layoutsSettings.activeInHierarchy);
        }*/

        /// <summary>
        /// Call animation which appear layouts settings popup.
        /// </summary>
        private void OnLayoutSettingsAppearing()
        {
            _layoutsSettings.SetActive(true);
            AppearWindow(_layoutsSettings);
        }

        /// <summary>
        /// Close <see cref="_statisticLayer"/>.
        /// </summary>
        public void OnClickLayoutsSettingsLayerCloseBtn()
        {
            //DisappearWindow(_layoutsSettings, OnWindowDisappeared);

            //void OnWindowDisappeared()
            //{
                //_layoutsSettings.SetActive(false);
                //AppearGameLayer();
            //}
        }

        public override void SetLevel(int level)
        {
            base.SetLevel(level);
            switch (level)
            {
                case 0:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[0];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 1:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[1];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[1];
                        //_buttonHint.SetActive(true);
                        break;
                    }
                case 2:
                    {
                        _undoPerformComponent.DefaultUndoCounts = _undoPerformComponent.DefaultUndoCountsLevels[2];
                        _hintManager.AvailableCountLevels = _hintManager.DefaultCountsLevels[0];
                        //_buttonHint.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Error 1265. Unknown level");
                        break;
                    }
            }
        }
    }
}