using System;
using Hanzo.Gfx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Ensure this namespace is included for UI components

public class SettingsManager : MonoBehaviour
{
    // public Button saveSettingsButton;

    // private const string QualityPrefKey = "GraphicsQuality";

    // public static SettingsManager Instance { get; private set; }

    // public GfxSettings gfxSettings;

    // private void Awake()
    // {

    //     // Ensure there's only one instance of SettingsManager
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject); // Optional: if you want this to persist across scenes
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    // public void Initialize(GfxSettings gfxSettings, Button saveButton)
    // {
    //     this.gfxSettings = gfxSettings;
    //     saveButton.onClick.AddListener(OnSaveButtonClick);
    // }

    // public float GetGraphicsQuality()
    // {
    //     return PlayerPrefs.GetFloat(QualityPrefKey, 1); // Default to 1 (Low) if no saved value
    // }

    // public void SetGraphicsQuality(float value)
    // {
    //     PlayerPrefs.SetFloat(QualityPrefKey, value);
    //     PlayerPrefs.Save(); // Ensure the change is written to disk
    // }

    // public void OnSaveButtonClick()
    // {
    //     if (gfxSettings != null)
    //     {
    //         float currentQuality = gfxSettings.GetCurrentQuality();
    //         SetGraphicsQuality(currentQuality);
    //         Debug.Log("Graphics settings saved.");
    //     }
    //     else
    //     {
    //         Debug.LogError("GfxSettings instance is null.");
    //     }
    // }

    // void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {

    //     if (SceneManager.GetActiveScene().name == "Game_Menu")
    //     {
    //         if (SceneManager.GetActiveScene().name == "Game_Menu")
    //         {
    //             gfxSettings = GameObject.FindObjectOfType<GfxSettings>();
    //             Debug.Log("GFX Assigned");
    //         }
    //     }
    // }


}

