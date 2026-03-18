using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; // Added to access TMP_Dropdown

public class StartMenu : MonoBehaviour
{
   public Slider sensitivitySlider;
   public Slider volumeSlider;
   public AudioMixer AudioMixer;
   
   // Added UI references for the dropdowns
   public TMP_Dropdown qualityDropdown; 
   public TMP_Dropdown resolutionDropdown;
   public PowerSystem powerSystem;

   public void Start()
   {
      SettingsData.CameraSensitivity = PlayerPrefs.GetFloat("SensitivityPreference", 0.7f);
      sensitivitySlider.value = SettingsData.CameraSensitivity; 

      float volume;
      AudioMixer.GetFloat("volume", out volume);
      volumeSlider.value = volume; 
   }
   
   public void StartClass()
   {
      SceneManager.LoadScene(1);
   }

   public void DropOut()
   {
      Application.Quit();
   }

   public void SetVolume(float volume)
   {
      AudioMixer.SetFloat("volume", volume);
   }

   public void SetSenstivity(float value)
   {
      SettingsData.CameraSensitivity = value;
      PlayerPrefs.SetFloat("SensitivityPreference", value); 
      PlayerPrefs.Save();
   }

   public void ResetToDefault()
   {
      // 1. Reset and save Sensitivity
      SettingsData.CameraSensitivity = 0.7f;
      PlayerPrefs.SetFloat("SensitivityPreference", 0.7f);

      // 2. Reset Volume
      AudioMixer.SetFloat("volume", 0f);

      // 3. Reset Graphics to highest (Index 2 for High)
      QualitySettings.SetQualityLevel(2, true);
      PlayerPrefs.SetInt("QualityPreference", 2);

      // 4. Reset Resolution to highest available on the monitor
      int highestResIndex = 0; // Keep track of the index for the UI
      Resolution[] availableResolutions = Screen.resolutions;
      
      if (availableResolutions.Length > 0)
      {
          highestResIndex = availableResolutions.Length - 1;
          Resolution highestRes = availableResolutions[highestResIndex];
          
          Screen.SetResolution(highestRes.width, highestRes.height, true);
          PlayerPrefs.SetInt("ResolutionPreference", highestResIndex);
      }

      PlayerPrefs.Save();

      // --- Update all the UI elements to reflect the newly applied defaults ---
      sensitivitySlider.value = 0.7f;
      volumeSlider.value = 0f;

      if (qualityDropdown != null)
      {
          qualityDropdown.value = 2; // 2 = High
          qualityDropdown.RefreshShownValue();
      }

      if (resolutionDropdown != null)
      {
          resolutionDropdown.value = highestResIndex;
          resolutionDropdown.RefreshShownValue();
      }
   }

   public void SetEasy()
   {
      SettingsData.Difficulty = 0;
      StartClass();
   }

   public void SetNormal()
   {
      SettingsData.Difficulty = 1;
      StartClass();
   }

   public void SetHard()
   {
      SettingsData.Difficulty = 2;
      StartClass();
   }

   public void SetMadness()
   {
      SettingsData.Difficulty = 3;
      StartClass();
   }
}