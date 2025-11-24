#if GAME_PUSH
using GamePush;
#endif
using SimpleSolitaire.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BloomLines.Helpers;

[System.Serializable]
public class LeaderboardFetchData
{
    public string avatar;
    public int id;
    public int score;
    public string name;
    public int position;

    public int gold;
    public int level;
}

public class TabLeaderboard : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Text _experienceIndicator;
    [SerializeField] private LBPlayer _thisPlayer;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private LBPlayer[] _lBPlayers;
    //[SerializeField] private Transform _itemsParent;
    //[SerializeField] private UILeaderboardItem _itemPrefab;
    private long _lastUpdateTimestamp;

    public int Rank { get; set; }

    private void OnEnable()
    {
        var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Mathf.Abs(currentTimestamp - _lastUpdateTimestamp) < 60f)
            return;

#if GAME_PUSH
        if (GP_Init.isReady)
        {
            _experienceIndicator.text = GP_Player.GetScore().ToString();        
        
            Debug.Log("GP_LB Fetching...");
            if (_gameManager.GamePush == null)
                Debug.LogError("_gameManager.GamePush == null");
            _gameManager.GamePush.FetchLeaderboard();
            _gameManager.GamePush.FetchPlayerRating();
            _loadingScreen.SetActive(true);
        }
        else
            Debug.Log("GP_LB is not ready");
#if UNITY_EDITOR
        for (int i = 0; i < 50; i++)
        {
            string playerName = "Player " + (i + 1).ToString();
            string playerScore = FormatNumbers.Format(100 + (50 - i) * 150); 
            string playerRank = (i + 1).ToString();
            string playerAvatar = "https://games.pikabu.ru/static/0/images/def_avatar/games.png";

            if (i < _lBPlayers.Length)
            {
                _lBPlayers[i].Set(playerName, playerScore, playerRank, playerAvatar);
            }
        }

        _thisPlayer.Set("Begemot", FormatNumbers.Format(5278000), "10", "https://games.pikabu.ru/static/0/images/def_avatar/games.png");
        _loadingScreen.SetActive(false);
#endif
#endif
    }

    private void OnOpen() => Debug.Log("LEADERBOARD: ON OPEN");
    private void OnClose() => Debug.Log("LEADERBOARD: ON CLOSE");

#if GAME_PUSH
    public void OnFetchSuccess(string fetchTag, GP_Data data)
    {
        Debug.Log("LEADERBOARD: OnFetchLBPlayers Success()");
        _loadingScreen.SetActive(false);
        SetPlayers(data);
    }

    public void OnFetchPlayerRatingSuccess(string fetchTag, int position)
    {
        Debug.Log("LEADERBOARD: OnFetchPlayerRating Success() " + fetchTag + " PLAYER POSITION: " + position);
        Rank = position;
        string playerName = GP_Player.GetName();
        string score = FormatNumbers.Format(GP_Player.GetScore());
        //string scoreForAllGames = PlayerPrefs.GetInt("Experience").ToString();        
        string avatarUrl = GP_Player.GetAvatarUrl();
        string rank = position.ToString();
        if (_thisPlayer != null)
            _thisPlayer.Set(playerName, score, rank, avatarUrl);   
        else
            Debug.Log("GP. ThisPlayer == null");
    }

    public void SetPlayers(GP_Data gp_data)
    {  
        if (gp_data == null)
        {
            Debug.LogError("GP_LB.Data == null. It is normal in Unity Editor");
            return;
        }

        //Debug.Log("GP_LeaderBoard is got success");
        List<LeaderboardFetchData> players = gp_data.GetList<LeaderboardFetchData>();
        Debug.Log("GP_LB: " + players.Count);

        for (int i = 0; i < players.Count; i++)
        {
            string playerName = players[i].name;
            string playerScore = players[i].score.ToString();
            string playerRank = players[i].position.ToString();
            string playerAvatar = players[i].avatar;
            Debug.Log("PLAYER: " + i);            
            Debug.Log("PLAYER.ID: " + players[i].id);
            Debug.Log("PLAYER.SCORE: " + playerScore);
            Debug.Log("PLAYER.NAME: " + playerName);
            Debug.Log("PLAYER.POSITION: " + playerRank);

            try
            {
                if (i < _lBPlayers.Length)
                {
                    _lBPlayers[i].Set(playerName, playerScore, playerRank, playerAvatar);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error Set PLAYER" + i + ": " + ex.Message);
            }            
        }
    }
#endif
}
