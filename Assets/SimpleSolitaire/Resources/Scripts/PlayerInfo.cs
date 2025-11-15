using UnityEngine;
using UnityEngine.UI;
#if GAME_PUSH
using GamePush;
#endif

namespace BloomLines
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private Text _title;
        [SerializeField] private Image _avatar;

        private void Start()
        {
#if GAME_PUSH && UNITY_WEBGL && !UNITY_EDITOR
            _title.text = GP_Player.GetName();
            GP_Player.GetAvatar(_avatar);
#endif
        }
    }
}
