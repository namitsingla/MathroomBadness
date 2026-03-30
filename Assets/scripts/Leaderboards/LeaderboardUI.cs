using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;

public class LeaderboardUI : MonoBehaviour
{
    [Header("References")]
    public LeaderboardManager manager; // Drag your manager here
    public GameObject rowPrefab;       // Drag your Row Prefab here
    public Transform contentPanel;     // The Vertical Layout Group container

    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark Grey
    public Color playerColor = new Color(0.8f, 0.6f, 0.1f, 1f); // Gold/Yellow

    public Color defaultBtnColor = Color.white; 
    public Color selectedBtnColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Slightly darker grey

    [Header("Difficulty Buttons")]
    public Button easyBtn;
    public Button normalBtn;
    public Button hardBtn;
    public Button madnessBtn;

    [Header("Difficulty Button Images")]
    public RawImage easyImage;
    public RawImage normalImage;
    public RawImage hardImage;
    public RawImage madnessImage;


    private void Start()
    {
        // Hook up the buttons to load the respective leaderboards
        easyBtn.onClick.AddListener(() => LoadLeaderboard(0));
        normalBtn.onClick.AddListener(() => LoadLeaderboard(1));
        hardBtn.onClick.AddListener(() => LoadLeaderboard(2));
        madnessBtn.onClick.AddListener(() => LoadLeaderboard(3));
    }

    // Call this method when your "Open Leaderboards" button is clicked
    public void OpenLeaderboardWindow()
    {
        // Load Normal difficulty by default
        LoadLeaderboard(1);
    }

    private async void LoadLeaderboard(int difficulty)
    {
        // Disable buttons while loading
        easyBtn.interactable = false;
        normalBtn.interactable = false;
        hardBtn.interactable = false;
        madnessBtn.interactable = false;

        // 1. Clear out the old rows from the previous difficulty
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. Fetch the data from the Manager
        var data = await manager.GetLeaderboardData(difficulty);
        var topScores = data.topScores;
        var playerScore = data.playerScore;

        if (topScores == null) return;

        // To get updated name insatntly
        string myPlayerId = AuthenticationService.Instance.PlayerId;
        bool amIInTop10 = false;
        
        // 1. GRAB THE FRESH NAME DIRECTLY FROM THE AUTH SERVICE
        string freshLocalName = await AuthenticationService.Instance.GetPlayerNameAsync();
        // Clean it up just in case it has the '#' tag
        if (!string.IsNullOrEmpty(freshLocalName) && freshLocalName.Contains("#")) {
            freshLocalName = freshLocalName.Split('#')[0];
        } else if (string.IsNullOrEmpty(freshLocalName)) {
            freshLocalName = "Anonymous";
        }

        // 3. Check if the current player is already in the Top 10 list

        foreach (var entry in topScores)
        {
            if (entry.PlayerId == myPlayerId)
            {
                amIInTop10 = true;
                break;
            }
        }

        // 4. Determine how many Top 10 rows to spawn. 
        // If we aren't in the top 10, we only spawn 9, saving the 10th spot for ourselves.
        int rowsToSpawn = amIInTop10 ? topScores.Count : Mathf.Min(topScores.Count, 9);

        for (int i = 0; i < rowsToSpawn; i++)
        {
            var entry = topScores[i];
            bool isMe = (entry.PlayerId == myPlayerId);
            
            string displayName = entry.PlayerName;

            // 2. THE SPOOF: If this row is ME, ignore the server's cached name and use the fresh one!
            if (isMe)
            {
                displayName = freshLocalName;
            }
            else 
            {
                // Normal cleanup for everyone else
                if (!string.IsNullOrEmpty(displayName) && displayName.Contains("#"))
                    displayName = displayName.Split('#')[0];
                else if (string.IsNullOrEmpty(displayName) || displayName.Length > 20)
                    displayName = "Anonymous"; 
            }

            SpawnRow((entry.Rank + 1).ToString(), displayName, ((int)entry.Score).ToString(), isMe);
        }

        // 5. Handle the bottom row if the player is NOT in the Top 10
        // 3. DO THE SAME SPOOF FOR THE BOTTOM ROW
        if (!amIInTop10)
        {
            if (playerScore != null)
            {
                // We don't even need to read playerScore.PlayerName, we just use the fresh one
                SpawnRow((playerScore.Rank + 1).ToString(), freshLocalName, ((int)playerScore.Score).ToString(), true);
            }
            else
            {
                SpawnRow("NA", freshLocalName, "-", true);
            }
        }

        // Re enable the buttons
        easyBtn.interactable = true;
        normalBtn.interactable = true;
        hardBtn.interactable = true;
        madnessBtn.interactable = true;

        HighlightDifficultyButton(difficulty);
    }

    private void SpawnRow(string rank, string name, string score, bool isPlayer)
    {
        GameObject newRow = Instantiate(rowPrefab, contentPanel, false);
        LeaderboardRow rowScript = newRow.GetComponent<LeaderboardRow>();
        
        Color bgColor = isPlayer ? playerColor : normalColor;
        rowScript.Setup(rank, name, score, bgColor);
    }

    private void HighlightDifficultyButton(int difficulty)
    {
        // Set the color based on which difficulty was selected
        easyImage.color = (difficulty == 0) ? selectedBtnColor : defaultBtnColor;
        normalImage.color = (difficulty == 1) ? selectedBtnColor : defaultBtnColor;
        hardImage.color = (difficulty == 2) ? selectedBtnColor : defaultBtnColor;
        madnessImage.color = (difficulty == 3) ? selectedBtnColor : defaultBtnColor;
    }

    [ContextMenu("TEST: Spawn 10 Fake Rows")]
    public void TestUILayout()
    {
        // 1. Clear out any existing rows
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // 2. Generate 10 fake players and scores
        int currentScore = 9500;
        for (int i = 1; i <= 10; i++)
        {
            // Make the score go down a bit each time so it looks realistic
            currentScore -= Random.Range(100, 800); 
            
            // Randomly make the 4th row "You" to test the gold color
            bool isMe = (i == 4); 
            string fakeName = isMe ? "My Test Name" : "Student_" + Random.Range(10, 99);

            SpawnRow(i.ToString(), fakeName, currentScore.ToString(), isMe);
        }
    }
}