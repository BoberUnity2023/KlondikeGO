using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenu : MonoBehaviour
{
    [SerializeField] private Button _buttonUp;
    [SerializeField] private Button _buttonDown;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void PressUp()
    {
        _animator.ResetTrigger("Down");
        _animator.SetTrigger("Up");
        _buttonUp.gameObject.SetActive(false);
        _buttonDown.gameObject.SetActive(true);
    }

    public void PressDown()
    {
        _animator.ResetTrigger("Up");
        _animator.SetTrigger("Down");
        _buttonUp.gameObject.SetActive(true);
        _buttonDown.gameObject.SetActive(false);
    }
}
