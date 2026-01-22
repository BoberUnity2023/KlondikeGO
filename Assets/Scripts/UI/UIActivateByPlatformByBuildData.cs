using BloomLines.Assets;
using UnityEngine;

namespace BloomLines
{
    public class UIActivateByPlatformByBuildData : MonoBehaviour
    {
        [SerializeField] private GameObject _logoText;

        private void Awake()
        {
            BuildData buildData = Resources.Load<BuildData>("BuildData");
            bool _isYandex = buildData.BuildPlatform == BuildPlatform.Yandex;
            _logoText.SetActive(!_isYandex);
        }
    }
}
