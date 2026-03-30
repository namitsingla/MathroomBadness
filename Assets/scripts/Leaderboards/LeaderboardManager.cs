using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Leaderboards.Models;

public class LeaderboardManager : MonoBehaviour
{

    // Keys used to save data locally if the internet drops
    private const string OFFLINE_SCORE_KEY = "PendingOfflineScore";
    private const string OFFLINE_DIFF_KEY = "PendingOfflineDifficulty";
    // To handle version changes
    private const string GAME_VERSION = "v1";

    public static LeaderboardManager Instance { get; private set; }

    private void Awake()
    {
        // If there is already an instance, and it's not this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Otherwise, make this the official instance and protect it from scene loads
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    async void Start()
    {
        // Start the connection process as soon as the GameObject loads
        await InitializeUGS();
    }

    private async Task InitializeUGS()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("UGS Initialized. Player ID: " + AuthenticationService.Instance.PlayerId);

            // Once connected, check if the player had a run that failed to upload previously
            CheckOfflineScores();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize UGS: " + e);
        }
    }

    // Helper to map the enum to your exact dashboard IDs
    private string GetLeaderboardId(int difficulty)
    {
        string baseId = "";

        switch (difficulty)
        {
            case 0: baseId = "scores_easy"; break;
            case 1: baseId =  "scores_normal"; break;
            case 2: baseId = "scores_hard";  break;
            case 3: baseId = "scores_madness"; break;
            default: baseId =  "scores_normal"; break;
        }

        // This will output exactly: "scores_normal_v1"
        return $"{baseId}_{GAME_VERSION}";
    }

    // -------------------------------------------------------------------
    // SUBMITTING SCORES & OFFLINE CACHING
    // -------------------------------------------------------------------

    public async Task SubmitScoreForDifficulty(int finalScore, int difficulty)
    {
        string targetLeaderboard = GetLeaderboardId(difficulty);

        try
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(targetLeaderboard, finalScore);
            Debug.Log($"Successfully submitted {finalScore} to {targetLeaderboard}. New rank: {scoreResponse.Rank + 1}");
            
            // If submission succeeds, clear any old offline data
            PlayerPrefs.DeleteKey(OFFLINE_SCORE_KEY);
            PlayerPrefs.DeleteKey(OFFLINE_DIFF_KEY);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Network error. Saving score offline: " + e.Message);
            SaveScoreOffline(finalScore, difficulty);
        }
    }

    private void SaveScoreOffline(int score, int difficulty)
    {
        // Note: This basic setup saves the single most recent failed score.
        // If they play 10 offline runs, it only keeps the last one.
        PlayerPrefs.SetInt(OFFLINE_SCORE_KEY, score);
        PlayerPrefs.SetInt(OFFLINE_DIFF_KEY, difficulty);
        PlayerPrefs.Save();
        Debug.Log("Score saved locally to PlayerPrefs.");
    }

    private async void CheckOfflineScores()
    {
        if (PlayerPrefs.HasKey(OFFLINE_SCORE_KEY) && PlayerPrefs.HasKey(OFFLINE_DIFF_KEY))
        {
            int pendingScore = PlayerPrefs.GetInt(OFFLINE_SCORE_KEY);
            int pendingDiff = (int)PlayerPrefs.GetInt(OFFLINE_DIFF_KEY);

            Debug.Log($"Found offline score: {pendingScore} on {pendingDiff}. Attempting upload...");
            
            // Try to upload it again
            await SubmitScoreForDifficulty(pendingScore, pendingDiff);
        }
    }

    // -------------------------------------------------------------------
    // FETCHING SCORES
    // -------------------------------------------------------------------

    public async Task<(List<LeaderboardEntry> topScores, LeaderboardEntry playerScore)> GetLeaderboardData(int difficulty)
    {
        string targetLeaderboard = GetLeaderboardId(difficulty);
        List<LeaderboardEntry> topScores = new List<LeaderboardEntry>();
        LeaderboardEntry playerScore = null;

        try
        {
            // 1. Fetch Top 10
            var options = new GetScoresOptions { Limit = 10 };
            var response = await LeaderboardsService.Instance.GetScoresAsync(targetLeaderboard, options);
            topScores = response.Results;
        }
        catch (System.Exception e) { Debug.LogError("Failed to fetch top scores: " + e); }

        try
        {
            // 2. Fetch the current player's specific score
            playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(targetLeaderboard);
        }
        catch (Unity.Services.Leaderboards.Exceptions.LeaderboardsException e)
        {
            // The correct UGS enum for a missing player score is ScoreSubmissionRequired
            if (e.Reason == Unity.Services.Leaderboards.Exceptions.LeaderboardsExceptionReason.ScoreSubmissionRequired) 
            {
                Debug.Log($"Player has no score on {difficulty}.");
            } 
            else 
            {
                Debug.LogError("Error fetching player score: " + e);
            }
        }

        return (topScores, playerScore);
    }

    // -------------------------------------------------------------------
    // PLAYER PROFILE
    // -------------------------------------------------------------------

    public async Task SetPlayerName(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            Debug.Log("Player name updated to: " + newName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update name: " + e);
        }
    }
}