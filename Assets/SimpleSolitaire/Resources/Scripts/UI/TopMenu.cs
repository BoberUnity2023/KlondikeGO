using UnityEngine;

namespace BloomLines
{
    public class TopMenu : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private bool _isUp;

        private void Start()
        {            
            _isUp = true;
        }
        
        public void Show()
        {
            _isUp = false;
            _animator.ResetTrigger("Up");
            _animator.SetTrigger("Down");
        }

        public void Hide()
        {
            if (_isUp)
                return;

            _isUp = true;
            _animator.ResetTrigger("Down");
            _animator.SetTrigger("Up");
        }
    }
}
