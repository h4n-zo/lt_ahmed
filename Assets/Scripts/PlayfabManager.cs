using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    private const string GuestCustomIdKey = "GuestCustomId";
    private const string GuestUsernameKey = "GuestUsername"; // Store generated usernames locally




    private TextMeshProUGUI usernameText;
    private string guestUserID;

    public static PlayfabManager instance;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    #region Login As Guest
    public IEnumerator OnSignIn(float t)
    {
        yield return new WaitForSeconds(t);
        SignInAsGuest();
    }

    public void HandleLoginButtonClicked()
    {
        SignInAsGuest();

    }

    private void SignInAsGuest()
    {
        string customId = GetOrCreateCustomId();

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private string GetOrCreateCustomId()
    {
        if (PlayerPrefs.HasKey(GuestCustomIdKey))
        {
            return PlayerPrefs.GetString(GuestCustomIdKey);
        }
        else
        {
            string newCustomId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GuestCustomIdKey, newCustomId);
            PlayerPrefs.Save();
            return newCustomId;
        }
    }

    private string GenerateRandomUsername()
    {
        if (PlayerPrefs.HasKey(GuestUsernameKey))
        {
            return PlayerPrefs.GetString(GuestUsernameKey);
        }
        else
        {
            string randomUsername = "Guest_" + Random.Range(1000, 9999); // Example: Guest_1234
            PlayerPrefs.SetString(GuestUsernameKey, randomUsername);
            PlayerPrefs.Save();
            return randomUsername;
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        GameObject.FindObjectOfType<LoginManager>().isLoginAsGuestSuccess = true;

        TextMeshProUGUI statusText = GameObject.Find("statusText").GetComponent<TextMeshProUGUI>();
        LoginManager _loginManager = GameObject.FindObjectOfType<LoginManager>();
        StartCoroutine(_loginManager.TypeText(statusText, "Login As Guest Successful..."));

        // Debug.Log("Guest login successful!");
        // Debug.Log($"PlayFab ID: {result.PlayFabId}");

        // Fetch the existing username (DisplayName)
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), accountInfoResult =>
        {
            if (!string.IsNullOrEmpty(accountInfoResult.AccountInfo.TitleInfo.DisplayName))
            {
                // Use the existing DisplayName
                guestUserID = accountInfoResult.AccountInfo.TitleInfo.DisplayName;
                Debug.Log($"Existing Username Retrieved: {guestUserID}");
            }
            else
            {
                // Generate and upload a new username if DisplayName is not set
                guestUserID = GenerateRandomUsername();
                Debug.Log($"Generated New Username: {guestUserID}");
                UpdateUsernameOnPlayFab(guestUserID);
            }

            // Optionally update UI or proceed to the next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }, error =>
        {
            Debug.LogError("Error fetching account info:");
            Debug.LogError(error.GenerateErrorReport());

            // Handle error gracefully, perhaps fallback to generating a new username
            guestUserID = GenerateRandomUsername();
            UpdateUsernameOnPlayFab(guestUserID);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Error logging in as guest:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    private void UpdateUsernameOnPlayFab(string username)
    {
        var updateRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(updateRequest,
            result => Debug.Log($"Username '{username}' successfully updated in PlayFab."),
            error => Debug.LogError("Error updating username: " + error.GenerateErrorReport()));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Perform any scene-specific actions here, such as re-assigning UI buttons or resetting state
        if (scene.name.Equals("Game_Menu"))
        {
            // Find and update the usernameText UI element
            usernameText = GameObject.Find("usernameText").GetComponent<TextMeshProUGUI>();

            if (usernameText != null) // Ensure the component was found
            {
                usernameText.text = guestUserID;
            }
            else
            {
                Debug.LogError("usernameText GameObject not found");
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from scene loaded events when the object is destroyed
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    #region Login with email

    public void LoginWithEmail(string email, string password)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnEmailLoginSuccess, OnEmailLoginFailure);
    }

    private void OnEmailLoginSuccess(LoginResult result)
    {
        Debug.Log("Email login successful!");
        Debug.Log($"PlayFab ID: {result.PlayFabId}");

        StartCoroutine(LoginSuccessProcess(1.4f));

        // Fetch the existing username (DisplayName)
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), accountInfoResult =>
        {
            if (!string.IsNullOrEmpty(accountInfoResult.AccountInfo.TitleInfo.DisplayName))
            {
                // Use the existing DisplayName
                guestUserID = accountInfoResult.AccountInfo.TitleInfo.DisplayName;
                Debug.Log($"Existing Username Retrieved: {guestUserID}");
            }



        }, error =>
        {
            Debug.LogError("Error fetching account info:");
            Debug.LogError(error.GenerateErrorReport());

            // Handle error gracefully, perhaps fallback to generating a new username


        });

    }


    private void OnEmailLoginFailure(PlayFabError error)
    {
        Debug.LogError("Error logging in with email:");
        Debug.LogError(error.GenerateErrorReport());

        // Update UI with error message
        TextMeshProUGUI statusText = GameObject.FindObjectOfType<LoginManager>().statusText;
        statusText.text = "Email login failed. Please check your credentials.";
    }

    IEnumerator LoginSuccessProcess(float t)
    {
        LoginManager _loginManager = GameObject.FindObjectOfType<LoginManager>();
        _loginManager.OnLoginSuccess();

        yield return new WaitForSeconds(t);

        _loginManager.loadingBar.SetActive(false);
        TextMeshProUGUI statusText = GameObject.Find("statusText").GetComponent<TextMeshProUGUI>();
        StartCoroutine(_loginManager.TypeText(statusText, "Login With Email Successful..."));

        yield return new WaitForSeconds(t);
        // Proceed to the Game_Menu scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    #endregion

    #region Register User

    public void RegisterUser(string username, string email, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("User registration successful!");
        guestUserID = result.Username;
        UpdateUsernameOnPlayFab(guestUserID);

        StartCoroutine(RegisterSuccessProcess(1.5f));

        // TextMeshProUGUI registerStatusText = GameObject.FindObjectOfType<LoginManager>().registerStatusText;
        // LoginManager _loginManager = FindObjectOfType<LoginManager>();
        // StartCoroutine(_loginManager.TypeText(registerStatusText, "Registration successful! You can now log in."));



    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("Error registering user:");
        Debug.LogError(error.GenerateErrorReport());
        TextMeshProUGUI registerStatusText = GameObject.FindObjectOfType<LoginManager>().registerStatusText;
        registerStatusText.text = "Registration failed. Please try again.";
    }

    IEnumerator RegisterSuccessProcess(float t)
    {
        LoginManager _loginManager = GameObject.FindObjectOfType<LoginManager>();
        _loginManager.OnLoginSuccess();

        yield return new WaitForSeconds(t);

        _loginManager.loadingBar.SetActive(false);
        TextMeshProUGUI statusText = GameObject.Find("statusText").GetComponent<TextMeshProUGUI>();
        StartCoroutine(_loginManager.TypeText(statusText, "Registration successful! You can now log in."));

        yield return new WaitForSeconds(t);
        // Proceed to the Game_Menu scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    #endregion

    #region Password Reset
    public void ResetPassword(string email)
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = PlayFabSettings.staticSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordResetSuccess, OnPasswordResetFailure);
    }

    private void OnPasswordResetSuccess(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Password reset email sent successfully!");
        TextMeshProUGUI forgotPasswordStatusText = GameObject.FindObjectOfType<LoginManager>().forgotPasswordStatusText;
        forgotPasswordStatusText.text = "Password reset email sent. Check your inbox.";
    }

    private void OnPasswordResetFailure(PlayFabError error)
    {
        Debug.LogError("Error sending password reset email:");
        Debug.LogError(error.GenerateErrorReport());
        TextMeshProUGUI forgotPasswordStatusText = GameObject.FindObjectOfType<LoginManager>().forgotPasswordStatusText;
        forgotPasswordStatusText.text = "Failed to send password reset email. Please try again.";
    }

    #endregion

}

