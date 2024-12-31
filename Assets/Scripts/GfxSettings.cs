using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro; // Add this for TextMeshProUGUI
using UnityEngine.UI;

namespace Hanzo.Gfx
{
    public class GfxSettings : MonoBehaviour
    {
        public UniversalRenderPipelineAsset lowQualityAsset;
        public UniversalRenderPipelineAsset mediumQualityAsset;
        public UniversalRenderPipelineAsset highQualityAsset;

        public Slider qualitySlider; // Slider for adjusting quality
        public TextMeshProUGUI gfxText;

        private int currentQuality; // Changed to int for easier handling

        [SerializeField] MenuSelect menuSelect;

        private void Awake()
        {
            menuSelect = GameObject.FindObjectOfType<MenuSelect>();
        }

        private void Start()
        {
            // Load the saved quality value
            currentQuality = (int)menuSelect.gfxManager.GetGraphicsQuality();
            qualitySlider.value = currentQuality; // Set the slider to the current quality
            UpdateGraphicsText();
            ApplyGraphicsSettings(currentQuality);

            // Add slider listener
            qualitySlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            // Round the slider value to the nearest integer
            currentQuality = Mathf.RoundToInt(value);
            // Clamp the value to ensure it's within bounds
            currentQuality = Mathf.Clamp(currentQuality, 0, 2);
            UpdateGraphicsText();
            ApplyGraphicsSettings(currentQuality);
        }

        private void ApplyGraphicsSettings(int value)
        {
            switch (value)
            {
                case 0:
                    QualitySettings.SetQualityLevel(0); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = lowQualityAsset;
                    QualitySettings.globalTextureMipmapLimit = 2; // Set texture resolution to 512
                    Application.targetFrameRate = 30;
                    // Debug.Log("Graphics Quality: Low (512 Texture)");
                    // Debug.Log("CurrentQuality: " + currentQuality);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(1); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = mediumQualityAsset;
                    QualitySettings.globalTextureMipmapLimit = 1; // Set texture resolution to 1024
                    // QualitySettings.globalTextureMipmapLimit = 2; // Set texture resolution to 512
                    Application.targetFrameRate = 60;
                    // Debug.Log("Graphics Quality: Medium (1024 Texture)");
                    // Debug.Log("CurrentQuality: " + currentQuality);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(2); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = highQualityAsset;
                    // QualitySettings.globalTextureMipmapLimit = 0; // Set texture resolution to 4096
                    QualitySettings.globalTextureMipmapLimit = 1; // Set texture resolution to 2048 (2K)
                    Application.targetFrameRate = 60;
                    // Debug.Log("Graphics Quality: High (4096 Texture)");
                    // Debug.Log("CurrentQuality: " + currentQuality);
                    break;
            }
        }

        private void UpdateGraphicsText()
        {
            switch (currentQuality)
            {
                case 0:
                    gfxText.text = "LOW";
                    break;
                case 1:
                    gfxText.text = "MEDIUM";
                    break;
                case 2:
                    gfxText.text = "HIGH";
                    break;
            }
        }

        // Method to get the current graphics quality value
        public float GetCurrentQuality()
        {
            return currentQuality;
        }
    }
}
