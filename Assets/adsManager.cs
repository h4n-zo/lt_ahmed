using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

#if UNITY_ANDROID
    string appKey = "1f747a94d";
#elif UNITY_IPHONE
    string appKey = "1f747a94d";
#else
    string appKey = "unexpected_platform";
#endif

    public Button showInterstitialButton; // Assign these in the inspector
    public Button loadBannerButton; // Assign these in the inspector


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    // Start is called before the first frame update
    void Start()
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);
        LoadInterstitial();

        // ReassignButtonListeners();
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;


        IronSourceEvents.onSdkInitializationCompletedEvent += SDKInitialized;

        // Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        // Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Unsubscribe from events
        IronSourceEvents.onSdkInitializationCompletedEvent -= SDKInitialized;

        IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;

        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ReassignButtonListeners(); // Reassign button listeners after scene load
        StartCoroutine(EnableADS(3.5f));

    }

    IEnumerator EnableADS(float t)
    {
        yield return new WaitForSeconds(t);
        if (SceneManager.GetActiveScene().name == "Game_Menu")
        {
            LoadBanner();
            Debug.Log("EnableADS() is called");
        }

    }

    // private void ReassignButtonListeners()
    // {
    //     if (SceneManager.GetActiveScene().name == "Game_Menu")
    //     {
    //         // Reassign buttons if they lose their references
    //         if (showInterstitialButton == null || loadBannerButton == null)
    //         {
    //             showInterstitialButton = GameObject.Find("CreditsContainer")?.GetComponent<Button>();
    //             loadBannerButton = GameObject.Find("SettingsContainer")?.GetComponent<Button>();
    //         }

    //         if (showInterstitialButton != null)
    //         {
    //             showInterstitialButton.onClick.AddListener(ShowInterstitial);
    //         }

    //         if (loadBannerButton != null)
    //         {
    //             loadBannerButton.onClick.AddListener(LoadBanner);
    //         }
    //     }

    // }

    void SDKInitialized()
    {
        Debug.Log("Sdk is initialized!!");
    }

    void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }

    #region Banner
    public void LoadBanner()
    {
        Debug.Log("Clicked Banner");
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    // Banner AdInfo Delegates
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner loaded successfully.");
    }

    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        Debug.LogError($"Banner failed to load: {ironSourceError.getDescription()}");
    }

    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner clicked.");
    }

    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner screen presented.");
    }

    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Banner screen dismissed.");
    }

    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("User left the app after clicking the banner.");
    }
    #endregion

    #region Interstitial

    public void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    public void ShowInterstitial()
    {
        Debug.Log("Clicked Interstitial");
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("Interstitial not ready!");
            LoadInterstitial();
        }
    }

    // Interstitial AdInfo Delegates
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial ad ready.");
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.LogError($"Interstitial failed to load: {ironSourceError.getDescription()}");
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial ad opened.");
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial ad clicked.");
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.LogError($"Interstitial failed to show: {ironSourceError.getDescription()}");
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial ad closed.");
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("Interstitial ad show succeeded.");
    }

    #endregion
}
