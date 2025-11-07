using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LBPlayer : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _score;
    [SerializeField] private Text _rank;
    [SerializeField] private Image _avatar;

    public void Set(string name, string score, string rank, string url)
    {
        _name.text = name;

        if (_score != null)
            _score.text = score;

        if (_rank != null)
            _rank.text = rank;

        if (url.Length > 3)
            LoadAvatar(url);
    }    

    private void LoadAvatar(string url)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(LoadTextureFromWeb(url));
    }

    private IEnumerator LoadTextureFromWeb(string url)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(webRequest);
            _avatar.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
            _avatar.color = Color.white;
            //_avatar.SetNativeSize();
        }
        else
        {
            //Debug.LogError("Error: " + webRequest.error);
        }   
    }
}
