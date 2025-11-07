using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{    
    [SerializeField] private Text _counter;
    [SerializeField] private float _updateInterval = 0.2f;
    float _time = 0.0f;
    int _frames = 0;

    private void Start()
    {
        //_counter = GetComponent<Text>();
    }

    private void Update()
    {
        _time += Time.unscaledDeltaTime;
        ++_frames;

        if (_time >= _updateInterval)
        {
            float fps = (int)(_frames / _time);
            _time = 0.0f;
            _frames = 0;

            _counter.text = "FPS: " + fps.ToString();
        }
    }
}
