using UnityEngine;
using UnityEngine.UI;
using TMPro; // Include this if using TextMeshPro

public class ApplySavedVolume : MonoBehaviour
{
    public Button toggleVolumeButton; // Reference to the button
    public const string soundButtonTag = "soundButton"; // Tag for the button
    private bool isVolumeOn = true; // To track the current volume state

    void OnEnable()
    {
        // Find the button using the provided tag
        toggleVolumeButton = GameObject.Find(soundButtonTag)?.GetComponent<Button>();

        if (toggleVolumeButton != null)
        {
            // Set up the button listener
            toggleVolumeButton.onClick.AddListener(ToggleVolume);
            Debug.Log("Sound Button is assigned");
        }
        else
        {
            Debug.LogWarning("Toggle Volume Button is not assigned.");
        }
    }

    private void Start()
    {
        // Get the saved volume value from PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1); // Default to 1 (Max) if no saved value

        // Set the volume of the AudioListener
        AudioListener.volume = savedVolume;

        // Set initial volume state
        isVolumeOn = savedVolume > 0;

        if (AudioListener.volume == 0)
        {
            SetButtonText("SOUND:OFF");
        }
    }

    private void ToggleVolume()
    {
        if (isVolumeOn)
        {
            // Set volume to OFF
            AudioListener.volume = 0;
            PlayerPrefs.SetFloat("MusicVolume", 0);

            // Set button child text to "OFF"
            SetButtonText("SOUND:OFF");
        }
        else
        {
            // Set volume to 100%
            AudioListener.volume = 1;
            PlayerPrefs.SetFloat("MusicVolume", 1);

            // Set button child text back to "ON" or "100%"
            SetButtonText("SOUND:ON"); // Or use "100%" or any other desired text
        }

        // Save the changes
        PlayerPrefs.Save();

        // Toggle the state
        isVolumeOn = !isVolumeOn;
    }

    private void SetButtonText(string text)
    {
        // Assuming the text component is a child of the button
        TextMeshProUGUI buttonText = toggleVolumeButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            // If using the standard Text component instead of TextMeshPro
            Text uiText = toggleVolumeButton.GetComponentInChildren<Text>();
            if (uiText != null)
            {
                uiText.text = text;
            }
        }
    }
}
