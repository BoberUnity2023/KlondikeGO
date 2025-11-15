#if GAME_PUSH
using GamePush;
#endif
using SimpleSolitaire.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private LBPlayer _lBPlayer;
    [SerializeField] private LBPlayer[] _lBPlayers; 

    public int Rank { get; set; }

    private void OnEnable()
    {
#if GAME_PUSH
        if (GP_Init.isReady)
        {
            _experienceIndicator.text = GP_Player.GetScore().ToString();        
        
            Debug.Log("GP_LB Fetching...");
            _gameManager.GamePush.FetchLeaderboard();
            _gameManager.GamePush.FetchPlayerRating();
        }
        else
            Debug.Log("GP_LB is not ready");
#endif
    }

    private void OnOpen() => Debug.Log("LEADERBOARD: ON OPEN");
    private void OnClose() => Debug.Log("LEADERBOARD: ON CLOSE");

#if GAME_PUSH
    public void OnFetchSuccess(string fetchTag, GP_Data data)
    {
        Debug.Log("LEADERBOARD: OnFetchLBPlayers Success()");             
        SetPlayers(data);
    }

    public void OnFetchPlayerRatingSuccess(string fetchTag, int position)
    {
        Debug.Log("LEADERBOARD: OnFetchPlayerRating Success() " + fetchTag + " PLAYER POSITION: " + position);
        Rank = position;
        string playerName = GP_Player.GetName();
        string score = GP_Player.GetScore().ToString();
        string scoreForAllGames = PlayerPrefs.GetInt("Experience").ToString();        
        string avatarUrl = GP_Player.GetAvatarUrl();
        string rank = position.ToString();
        if (_lBPlayer != null)
            _lBPlayer.Set(playerName, score, rank, avatarUrl);   
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
            //Debug.Log("PLAYER.GOLD: " + players[i].gold);
            //Debug.Log("PLAYER.LEVEL: " + players[i].level);
            //Debug.Log("PLAYER.AVATAR: " + players[i].avatar);
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
