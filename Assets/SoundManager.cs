using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    private AudioListener audioListener;

    private const string VolumePrefKey = "MusicVolume";

    private void Start()
    {
        // Get the AudioListener component from the main camera
        audioListener = FindObjectOfType<AudioListener>();

        // Load saved volume value and set the slider
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1); // Default to 1 (Max) if no saved value
        volumeSlider.value = savedVolume;
        UpdateVolumeText();
        AudioListener.volume = savedVolume;

        // Add slider listener
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Decrease volume in increments of ten
        float newVolume = Mathf.Round(value * 10f) / 10f;
        if (newVolume == 0)
        {
            volumeText.text = "OFF";
        }
        else
        {
            volumeText.text = $"{(int)(newVolume * 100)}%";
        }
        AudioListener.volume = newVolume;
        
        // Save volume value
        PlayerPrefs.SetFloat(VolumePrefKey, newVolume);
        PlayerPrefs.Save();
    }

    private void UpdateVolumeText()
    {
        // Update the text based on the initial slider value
        float volume = volumeSlider.value;
        if (volume == 0)
        {
            volumeText.text = "OFF";
        }
        else
        {
            volumeText.text = $"{(int)(volume * 100)}%";
        }
    }
}
