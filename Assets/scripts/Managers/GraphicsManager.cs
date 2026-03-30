using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphicsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    // We changed this from an array to a List so we can easily add/remove items
    private List<Resolution> filteredResolutions; 

    void Start()
    {
        Screen.fullScreen = true;

         Application.targetFrameRate = 60;

        // --- QUALITY SETTINGS STARTUP ---
        int savedQuality = PlayerPrefs.GetInt("QualityPreference", 2);
        SetQuality(savedQuality);
        if (qualityDropdown != null)
        {
            qualityDropdown.SetValueWithoutNotify(savedQuality);
        }

        // --- RESOLUTION STARTUP ---
        Resolution[] allResolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        // 1. Calculate the player's native monitor aspect ratio (e.g., 16:9 is ~1.77)
        float nativeAspect = (float)Screen.currentResolution.width / Screen.currentResolution.height;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            float resAspect = (float)allResolutions[i].width / allResolutions[i].height;

            // 2. Only keep resolutions that match the monitor's shape (prevents stretching)
            if (Mathf.Abs(resAspect - nativeAspect) < 0.05f) 
            {
                string option = allResolutions[i].width + " x " + allResolutions[i].height;

                // 3. Filter out duplicates caused by different refresh rates
                if (!options.Contains(option))
                {
                    filteredResolutions.Add(allResolutions[i]);
                    options.Add(option);
                }
            }
        }

        // 4. If the list is still too long, trim it down to just the top 4 highest resolutions
        int maxOptions = 4;
        if (options.Count > maxOptions)
        {
            int itemsToRemove = options.Count - maxOptions;
            options.RemoveRange(0, itemsToRemove);
            filteredResolutions.RemoveRange(0, itemsToRemove);
        }

        // Set the default index to the highest available resolution in our new short list
        currentResIndex = filteredResolutions.Count - 1; 

        resolutionDropdown.AddOptions(options);

        // Load saved resolution, ensuring the saved index hasn't gone out of bounds of our new smaller list
        int savedResIndex = PlayerPrefs.GetInt("ResolutionPreference", currentResIndex);
        if (savedResIndex >= filteredResolutions.Count) 
        {
            savedResIndex = filteredResolutions.Count - 1;
        }

        SetResolution(savedResIndex);
        
        if (resolutionDropdown != null)
        {
            resolutionDropdown.SetValueWithoutNotify(savedResIndex);
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true); 
        PlayerPrefs.SetInt("QualityPreference", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        // Pull from our new filtered list instead of the raw Unity array
        Resolution res = filteredResolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, true);
        
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
        PlayerPrefs.Save();
    }
}