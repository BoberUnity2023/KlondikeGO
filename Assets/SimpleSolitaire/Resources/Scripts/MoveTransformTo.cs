using SimpleSolitaire.Controller;
using System.Collections;
using UnityEngine;

public class MoveTransformTo: MonoBehaviour
{
    [SerializeField] private AudioController _audioController;
    private Vector3 _startPosition = Vector3.zero;
    private Vector3 _finishPosition = Vector3.zero;

    private float _progress = 0;
    private bool _isMoving = false;    
    private float _speed = 0;
    [SerializeField] private bool _scaling;
    [SerializeField] private bool _waitMoving;

    public Vector3 FinishPosition => _finishPosition;

    private void FixedUpdate()
    {  
        if (_waitMoving)
        {
            transform.position = _startPosition;            
        }

        if (_isMoving)
        {
            _progress += Time.fixedDeltaTime * _speed;
            Vector3 rotateEffect = Vector3.right * Mathf.Sin(_progress * Mathf.PI * 1) * 10;
            Vector3 position = Vector3.Lerp(_startPosition, _finishPosition, _progress * _progress) + rotateEffect;
            transform.position = position;
            transform.SetAsLastSibling();

            if (_scaling)
                transform.localScale = Vector3.one * _progress;

            if (_progress > 1)
            {
                _isMoving = false;
                Finish();
            }
        }        
    }

    private void OnDisable()
    {
        if (_isMoving || _waitMoving)
        {
            _waitMoving = false;
            StopAllCoroutines();
            Finish(); 
        }
    }

    public void SetStartPosition(Vector3 startPosition)
    {
        _startPosition = startPosition;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetMoveTo(Vector3 finishPosition, float speed = 0, float delay = 0, bool scaling = false)
    {           
        if (_isMoving)
        {
            Debug.LogWarning("SetMoveTo(" + gameObject.name + ") Объект уже движется");
            return;
        }
        if (gameObject.activeInHierarchy)
            StartCoroutine(StartMove(delay));
        else
        { 
            transform.position = _finishPosition;
            return;
        }
        _waitMoving = delay > 0;
        _startPosition = transform.position;
        _finishPosition = finishPosition;
        _speed = speed;
        _progress = 0;
        _scaling = scaling;
    }

    private IEnumerator StartMove(float time)
    {
        yield return new WaitForSeconds(time);
        
        _isMoving = true;
        _waitMoving = false;
    }

    private void Finish()
    {
        _isMoving = false; 
        _progress = 0;
        transform.position = _finishPosition;   

        if (_scaling)
            transform.localScale = Vector3.one;

        _audioController.Play(AudioController.AudioType.CardPut);        
    }
}

