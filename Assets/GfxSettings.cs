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

        public Button leftButton;
        public Button rightButton;
        public TextMeshProUGUI gfxText;

        private int currentQuality; // Changed to int for easier handling

        [SerializeField] MenuSelect menuSelect;

        private void Start()
        {
            // menuSelect = GameObject.FindObjectOfType<MenuSelect>();
            // Load the saved quality value
            currentQuality = (int)menuSelect.gfxManager.GetGraphicsQuality();
            UpdateGraphicsText();
            ApplyGraphicsSettings(currentQuality);

            // Add button listeners
            leftButton.onClick.AddListener(OnLeftButtonClicked);
            rightButton.onClick.AddListener(OnRightButtonClicked);
        }

        private void OnLeftButtonClicked()
        {
            currentQuality--;
            if (currentQuality < 1)
            {
                currentQuality = 3; // Loop back to High if going below Low
            }
            ApplyGraphicsSettings(currentQuality);
            UpdateGraphicsText();
        }

        private void OnRightButtonClicked()
        {
            currentQuality++;
            if (currentQuality > 3)
            {
                currentQuality = 1; // Loop back to Low if going above High
            }
            ApplyGraphicsSettings(currentQuality);
            UpdateGraphicsText();
        }

        private void ApplyGraphicsSettings(int value)
        {
            switch (value)
            {
                case 1:
                    QualitySettings.SetQualityLevel(0); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = lowQualityAsset;
                    Application.targetFrameRate = 30;
                    Debug.Log("Graphics Quality : Low");
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(1); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = mediumQualityAsset;
                    Application.targetFrameRate = 60;
                    Debug.Log("Graphics Quality : Medium");
                    break;
                case 3:
                    QualitySettings.SetQualityLevel(2); // Optional: If using Unity's quality settings
                    GraphicsSettings.renderPipelineAsset = highQualityAsset;
                    Application.targetFrameRate = 60;
                    Debug.Log("Graphics Quality : High");
                    break;
            }
        }

        private void UpdateGraphicsText()
        {
            switch (currentQuality)
            {
                case 1:
                    gfxText.text = "GFX QUALITY : LOW";
                    break;
                case 2:
                    gfxText.text = "GFX QUALITY : MEDIUM";
                    break;
                case 3:
                    gfxText.text = "GFX QUALITY : HIGH";
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
