using UnityEngine;

namespace BloomLines
{
    public class LogoCorner : MonoBehaviour
    {        
        [SerializeField] private Animator _animator;
        private bool _isUp;

        public void Show()
        {
            if (_isUp)
                return;

            _isUp = true;
            _animator.ResetTrigger("Hide");
            _animator.SetTrigger("Show");
        }

        public void Hide()
        {
            if (!_isUp)
                return;

            _isUp = false;
            _animator.ResetTrigger("Show");
            _animator.SetTrigger("Hide");
        }
    }
}
