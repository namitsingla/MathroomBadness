using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Slider sensitivitySlider;
   public Slider volumeSlider;
   public AudioMixer AudioMixer;

   public void Start()
   {
      sensitivitySlider.value = SettingsData.CameraSensitivity; //update sliders when reentering main menu

      float volume;
      AudioMixer.GetFloat("volume", out volume);
      volumeSlider.value = volume; //update sliders when reentering main menu
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
   }

   public void ResetToDefault()
   {
      SettingsData.CameraSensitivity = 0.7f;
      AudioMixer.SetFloat("volume", 0f);

      // Update the slider's UI
      sensitivitySlider.value = 0.7f;
      volumeSlider.value = 0f;
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
