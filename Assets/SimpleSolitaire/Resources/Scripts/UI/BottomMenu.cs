using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenu : MonoBehaviour
{
    [SerializeField] private Button _buttonUp;
    [SerializeField] private Button _buttonDown;
    [SerializeField] private Animator _animator;
    private bool _isUp;

    private void Start()
    {        
        _isUp = false;
    }

    public void PressUp()
    {
        if (_isUp)
            return;

        _isUp = true;
        _animator.ResetTrigger("Down");
        _animator.SetTrigger("Up");
        _buttonUp.gameObject.SetActive(false);
        _buttonDown.gameObject.SetActive(true);
    }

    public void PressDown()
    {
        if (!_isUp)
            return;

        _isUp = false;
        _animator.ResetTrigger("Up");
        _animator.SetTrigger("Down");
        _buttonUp.gameObject.SetActive(true);
        _buttonDown.gameObject.SetActive(false);
    }
}
