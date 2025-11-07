using System.Collections.Generic;
using BloomLines.Controllers;
using BloomLines.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloomLines.UI
{
    public class UILeaderboardItem : MonoBehaviour
    {
        [SerializeField] private RawImage _photo;
        [SerializeField] private Texture2D _defaultPhoto;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _position;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private Image _background;
        [SerializeField] private Sprite[] _backgrounds;
        [SerializeField] private Color[] _textColors;

        public void Set(LeaderboardPlayerData playerData)
        {
            _name.text = string.IsNullOrEmpty(playerData.Name) ? "-" : playerData.Name;
            _position.text = playerData.Position.ToString();
            _score.text = FormatNumbers.Format(playerData.Score);

            _photo.texture = _defaultPhoto;
            //if(!string.IsNullOrEmpty(playerData.Photo))//откл. т.к. на моб. устройствах не работал скролл и кнопка закрытия окна
            //    ImageLoad.Load(playerData.Photo, OnPhotoLoaded);

            int index = Mathf.Clamp(playerData.Position - 1, 0, 3);

            _background.sprite = _backgrounds[index];
            _name.color = _textColors[index];
            _position.color = _textColors[index];
            _score.color = _textColors[index];
        }

        private void OnPhotoLoaded(Texture2D texture)
        {
            _photo.texture = texture;
        }
    }
}