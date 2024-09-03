using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Hanzo.Gfx;

public class MenuSelect : MonoBehaviour
{
    [System.Serializable]
    public struct GfxManager
    {

        public const string QualityPrefKey = "GraphicsQuality";

        public GfxSettings gfxSettings;

        public void EnableGFX()
        {

            gfxSettings = GameObject.FindObjectOfType<GfxSettings>();
            Debug.Log("GFX Assigned");

        }

        public float GetGraphicsQuality()
        {
            return PlayerPrefs.GetFloat(QualityPrefKey, 1); // Default to 1 (Low) if no saved value

        }

        public void SetGraphicsQuality(float value)
        {
            PlayerPrefs.SetFloat(QualityPrefKey, value);
            PlayerPrefs.Save(); // Ensure the change is written to disk
        }

        public void OnSaveButtonClick()
        {
            if (gfxSettings != null)
            {
                float currentQuality = gfxSettings.GetCurrentQuality();
                SetGraphicsQuality(currentQuality);
                Debug.Log("Graphics settings saved.");
            }
            else
            {
                Debug.LogError("GfxSettings instance is null.");
            }
        }
    }

    public GfxManager gfxManager;

    public CinemachineVirtualCamera[] virtualCameras;
    public GameObject[] canvasGO;
    private int currentIndex = 0; // Current camera index

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance to be considered a swipe

    private SettingsManager settingsManager;
    public Button saveButton;

    [Header("Character Select Functionality")]
    public UnityEvent _triggerSelect;
    public UnityEvent _triggerUnselect;

    [Header("Shop Select Functionality")]
    public UnityEvent _triggerShopSelect;
    public UnityEvent _triggerShopUnselect;

    [Header("Mission Select Functionality")]
    public UnityEvent _triggerMissionSelect;
    public UnityEvent _triggerMissionUnselect;

    [Header("Credits Functionality")]
    public UnityEvent _triggerCreditsSelect;
    public UnityEvent _triggerCreditsUnselect;

    [Header("Setings Functionality")]
    public UnityEvent _triggerSettingsSelect;
    public UnityEvent _triggerSettingsUnselect;

    void Start()
    {
        // Delete();
        UpdateCamera();
        Time.timeScale = 1f;
        gfxManager.EnableGFX();
    }

    private void Delete()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Data deleted");
    }

    void Update()
    {
        HandleSwipe();
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #region Character Select Methods
    public void TriggerCharacterSelect()
    {
        _triggerSelect?.Invoke();
    }

    public void TriggerCharacterUnselect()
    {
        _triggerUnselect?.Invoke();
    }
    #endregion

    #region Shop Select Methods
    public void TriggerShopSelect()
    {
        _triggerShopSelect?.Invoke();
    }

    public void TriggerShopUnselect()
    {
        _triggerShopUnselect?.Invoke();
    }

    #endregion

    #region Mission Select Methods
    public void TriggerMissionSelect()
    {
        _triggerMissionSelect?.Invoke();
    }

    public void TriggerMissionUnselect()
    {
        _triggerMissionUnselect?.Invoke();
    }

    #endregion

    #region Credits Methods
    public void TriggerCreditSelect()
    {
        _triggerCreditsSelect?.Invoke();
    }

    public void TriggerCreditUnselect()
    {
        // ResetCamera();
        _triggerCreditsUnselect?.Invoke();
    }

    #endregion

    #region Settings Methods
    public void TriggerSettingsSelect()
    {
        _triggerSettingsSelect?.Invoke();
    }

    public void TriggerSettingsUnselect()
    {
        // ResetCamera();
        _triggerSettingsUnselect?.Invoke();

    }

    #endregion


    public void OnRightButton()
    {
        currentIndex++;
        if (currentIndex >= virtualCameras.Length)
        {
            currentIndex = 0;
        }
        UpdateCamera();
    }

    public void OnLeftButton()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = virtualCameras.Length - 1;
        }
        UpdateCamera();
    }

    void UpdateCamera()
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == currentIndex);
            canvasGO[i].gameObject.SetActive(i == currentIndex);
        }
    }

    void ResetCamera()
    {
        Debug.Log("Reset Camera Called");
        currentIndex = 0;

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == currentIndex);
            canvasGO[i].gameObject.SetActive(i == currentIndex);
        }
    }

    void HandleSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = (Vector2)Input.mousePosition;
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        if (Vector2.Distance(startTouchPosition, endTouchPosition) >= minSwipeDistance)
        {
            Vector2 swipeDirection = endTouchPosition - startTouchPosition;
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0)
                {
                    // OnRightButton();
                    OnLeftButton();
                }
                else
                {
                    // OnLeftButton();
                    OnRightButton();
                }
            }
        }
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        settingsManager = GameObject.FindObjectOfType<SettingsManager>();
    }

    public void SaveGFXButton()
    {
        gfxManager.OnSaveButtonClick();

        // StartCoroutine(TimeToSaveGFx(5f));
    }

    public void AssignGFXButton()
    {
        // saveButton.onClick.AddListener(gfxManager.OnSaveButtonClick);
        // Debug.Log("gfxManager.OnSaveButtonClick() is assigned");
        // StartCoroutine(TimeToSaveGFx(5f));
    }





}
