using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class PlayerProfileUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInputField;
    public Button saveButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI placeholderText;

    // OnEnable runs every time this UI GameObject is turned on
    private async void OnEnable()
    {
        statusText.text = ""; 
        saveButton.onClick.RemoveAllListeners(); // Prevent duplicate clicks
        saveButton.onClick.AddListener(AttemptSaveName);

        // Fetch the name when the window opens
        await LoadCurrentName();
    }

    private async Task LoadCurrentName()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized) return;

            string currentName = await AuthenticationService.Instance.GetPlayerNameAsync();

            if (!string.IsNullOrEmpty(currentName))
            {
                // STRIP THE '#' HERE:
                if (currentName.Contains("#"))
                {
                    currentName = currentName.Split('#')[0];
                }
                
                nameInputField.text = currentName;
            }
            else
            {
                nameInputField.text = ""; 
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could not fetch player name: " + e.Message);
        }
    }

    private async void AttemptSaveName()
    {
        string requestedName = nameInputField.text.Trim();

        if (string.IsNullOrWhiteSpace(requestedName))
        {
            statusText.text = "Name cannot be empty!";
            statusText.color = Color.red;
            return;
        }

        if (requestedName.Length < 3)
        {
            statusText.text = "Atleast 3 characters.";
            statusText.color = Color.red;
            return;
        }

        if (ContainsProfanity(requestedName))
        {
            statusText.text = "Inappropriate!";
            statusText.color = Color.red;
            return; // Stop the code right here, do not upload to server!
        }

        saveButton.interactable = false;
        statusText.color = Color.red;
        statusText.text = "Saving to server...";

        try
        {
            // BYPASS THE MANAGER AND TALK DIRECTLY TO THE AUTH SERVICE:
            await AuthenticationService.Instance.UpdatePlayerNameAsync(requestedName);
            
            statusText.color = Color.green;
            statusText.text = "Name updated!";
        }
        catch (AuthenticationException e)
        {
            // THIS WILL NOW PROPERLY CATCH RATE LIMITS AND SERVER REJECTIONS
            statusText.color = Color.red;
            statusText.text = "Error: Name rejected or rate limited.";
            Debug.LogError("UGS Name Save Error: " + e.Message);
        }
        catch (RequestFailedException e)
        {
            statusText.color = Color.red;
            statusText.text = "Failed to connect to server.";
            Debug.LogError("UGS Name Save Error: " + e.Message);
        }
        finally
        {
            saveButton.interactable = true;
        }
    }

    private bool ContainsProfanity(string name)
    {
        // 1. Convert the name to lowercase so "BadWord" and "BADWORD" are caught equally
        string lowerName = name.ToLower();

        // 2. Your Blacklist (You can add as many words here as you want)
        string[] bannedWords = new string[] 
        { 
            // Standard Profanity (Expand this list yourself later!)
            "fuck", "shit", "bitch", "asshole", "cunt", "nigger", "nigga", "whore", "slut", "fag",

            "namit"
        };

        // 3. Check if the player's name contains ANY of the banned words
        foreach (string badWord in bannedWords)
        {
            if (lowerName.Contains(badWord))
            {
                return true; // We found a bad word!
            }
        }

        return false; // The name is clean
    }
}