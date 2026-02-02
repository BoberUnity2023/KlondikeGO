using SimpleSolitaire.Controller;
using SimpleSolitaire.Screen;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TopPanelLines
{
    One,
    Two,
}

public class ScreenTopPanelController : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private float _changeAspectRatio;
    [SerializeField] private ScreenOrientationPosition[] _screenOrientationPositions;
    [SerializeField] private ScreenOrientationScale[] _screenOrientationScales;
    [SerializeField] private float _a;
    private TopPanelLines _topPanelLines = TopPanelLines.One;
    private bool _isMobile;

    public void Init(bool isMobile)
    {
        //_isMobile = isMobile;
        foreach (var item in _screenOrientationPositions)
        {
            item.PositionHorizontal = item.Transform.localPosition;
        }

        foreach (var item in _screenOrientationScales)
        {
            item.ScaleHorizontal = item.Transform.localScale;
        }        
    }

    public void SetAspectRatio(float aspectRatio)
    {
        _a = aspectRatio;
        if (aspectRatio > _changeAspectRatio && _topPanelLines == TopPanelLines.One)
            SetLinesTwo();

        if (aspectRatio < _changeAspectRatio && _topPanelLines == TopPanelLines.Two)
            SetLinesOne();
    }

    private void SetLinesOne()
    {
        _topPanelLines = TopPanelLines.One;
        foreach (var item in _screenOrientationPositions)
        {
            item.Transform.localPosition = item.PositionHorizontal;            
        }

        foreach (var item in _screenOrientationScales)
        {
            item.Transform.localScale = item.ScaleHorizontal;
        }
    }

    private void SetLinesTwo()
    {
        _topPanelLines = TopPanelLines.Two;
        foreach (var item in _screenOrientationPositions)
        {  
            if (_isMobile && item.PositionVericalMobile != null)
                item.Transform.localPosition = item.PositionVericalMobile.localPosition;//Not used   
            else
                item.Transform.localPosition = item.PositionVerical.localPosition;//Used
        }

        foreach (var item in _screenOrientationScales)
        {
            item.ScaleHorizontal = item.Transform.localScale;
            item.Transform.localScale = Vector3.one * item.ScaleVerical;
        }
    }
}
