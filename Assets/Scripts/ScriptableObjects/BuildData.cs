using UnityEngine;

namespace BloomLines.Assets
{
    public enum BuildPlatform
    {
        Yandex,
        VK,
        OK,
        RuStore,
        CrazyGames,
        GD,
        Poki,
        GooglePlay
    }

    [CreateAssetMenu(fileName = "BuildData", menuName = "BloomLines/BuildData")]
    public class BuildData : ScriptableObject
    {
        [SerializeField] private BuildPlatform _buildPlatform;
        [SerializeField] private bool _console;

        public BuildPlatform BuildPlatform => _buildPlatform;
        public bool Console => _console;
    }
}