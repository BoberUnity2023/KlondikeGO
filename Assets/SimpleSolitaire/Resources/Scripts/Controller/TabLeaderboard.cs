#if GAME_PUSH
using GamePush;
#endif
using SimpleSolitaire.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BloomLines.Helpers;
using YG;
using YG.Utils.LB;
using BloomLines.Controllers;
using Platform = SimpleSolitaire.Controller.Platform;

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
    [SerializeField] private Text _experienceIndicatorYandex;
    [SerializeField] private LBPlayer _thisPlayerGamePush;
    [SerializeField] private GameObject _thisPlayerYandex;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private LBPlayer[] _lBPlayers;
    private long _lastUpdateTimestamp;
    private bool _isInited;
  
    public int Rank { get; set; }

    private void Awake()
    {
#if Yandex
        InitializeYandex();      
#endif
    }

    private void OnEnable()
    {
        string experience = FormatNumbers.Format(_gameManager.Save.Experience);
        _experienceIndicator.text = experience;
        _experienceIndicatorYandex.text = experience;

        var currentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        if (Mathf.Abs(currentTimestamp - _lastUpdateTimestamp) < 60f)
            return;

        _loadingScreen.SetActive(true);

#if Yandex
        GetLeaderboard();
#endif

#if GAME_PUSH
#if !UNITY_EDITOR
        if (GP_Init.isReady)
        {
            Debug.Log("GP_LB Fetching...");
            if (_gameManager.GamePush == null)
                Debug.LogError("_gameManager.GamePush == null");
            _gameManager.GamePush.FetchLeaderboard();
            _gameManager.GamePush.FetchPlayerRating();
            _loadingScreen.SetActive(true);
            SetThisPlayer();
        }
        else
            Debug.Log("GP_LB is not ready");
#endif
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

        _thisPlayerGamePush.Set("Begemot", FormatNumbers.Format(5278000), "10", "https://games.pikabu.ru/static/0/images/def_avatar/games.png");
        _loadingScreen.SetActive(false);
#endif
#endif
    }

    private void OnDestroy()
    {
#if Yandex
        if ( _isInited)
        {
            YG2.onGetLeaderboard -= OnGetLeaderboardYandex;
        }        
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
        string score = FormatNumbers.Format(_gameManager.Save.Experience);
        string avatarUrl = "";
        if (_gameManager.Device == Device.Desktop)
            avatarUrl = GP_Player.GetAvatarUrl();

        string rank = position.ToString();
        _thisPlayerGamePush.Set(playerName, score, rank, avatarUrl); 
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
            string playerScore = FormatNumbers.Format(players[i].score);
            string playerRank = players[i].position.ToString();
            string playerAvatar = players[i].avatar;
            if (_gameManager.Device == Device.Mobile)
            {
                playerAvatar = "";
            }
            //Debug.Log("PLAYER: " + i);            
            //Debug.Log("PLAYER.ID: " + players[i].id);
            //Debug.Log("PLAYER.SCORE: " + playerScore);
            //Debug.Log("PLAYER.NAME: " + playerName);
            //Debug.Log("PLAYER.POSITION: " + playerRank);

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

#if Yandex  
    public void InitializeYandex()
    {
        _isInited = true;
        YG2.onGetLeaderboard += OnGetLeaderboardYandex;
        GetLeaderboard();
        SetThisPlayer();
        Debug.Log("YandexLB Fetching...");
    }

    public void GetLeaderboard()
    {
        Debug.Log("Yandex GetLeaderboard");
        YG2.GetLeaderboard("score", 50, 50, "128x128");
    }

    public void SetScore(int score)
    {
        Debug.Log("Yandex SetLeaderboardScore: " + score);
        YG2.SetLeaderboard("score", score);
    }

    private void OnGetLeaderboardYandex(LBData lbData)
    {
        Debug.Log("Yandex OnLeaderboardLoaded");
        _loadingScreen.SetActive(false);

        var data = new LeaderboardData();
        data.Players = new LeaderboardPlayerData[lbData.players.Length];

        for (int i = 0; i < data.Players.Length; i++)
        {
            string playerName = lbData.players[i].name;
            int playerRank = lbData.players[i].rank;
            string playerScore = FormatNumbers.Format(lbData.players[i].score);
            string playerAvatar = lbData.players[i].photo;
            //TODO: ????? Temp
            if (_gameManager.Device == Device.Mobile)
            {
                playerAvatar = "";
            }
            _lBPlayers[i].Set(playerName, playerScore, playerRank.ToString(), playerAvatar);
        }
    }
#endif

    private void SetThisPlayer()
    {
        Platform platform = _gameManager.Platform;
        _thisPlayerYandex.SetActive(platform == Platform.Yandex);
        bool isPlatformGamePush = platform == Platform.VK || platform == Platform.Ok;
        _thisPlayerGamePush.gameObject.SetActive(isPlatformGamePush);
    }
}
