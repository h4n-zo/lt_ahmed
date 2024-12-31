using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public enum LoginType
    {
        Email,
        Guest
    }

    public LoginType loginType;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button loginWithEmailButton;
    public GameObject loadingBar;

    [Header("Login With Email Section")]
    public GameObject loginPanel;
    public GameObject registerContainer;
    public GameObject loginContainer;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    public TextMeshProUGUI statusText;

    [Header("Forgot Password Section")]
    [SerializeField] private GameObject forgotPasswordPanel;
    [SerializeField] private TMP_InputField forgotEmailInputField;
    public TextMeshProUGUI forgotPasswordStatusText;
    [SerializeField] private Button forgotPasswordButton;

    [Header("Register User Section")]
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private TMP_InputField registerUsernameInputField;
    [SerializeField] private TMP_InputField registerEmailInputField;
    [SerializeField] private TMP_InputField registerPasswordInputField;
    public TextMeshProUGUI registerStatusText;

    public bool isLoginAsGuestSuccess = false;
    public bool isRegisterSuccess = false;

    private void Start()
    {
        // PlayerPrefs.DeleteAll();

        if (PlayerPrefs.HasKey("GuestCustomId"))
        {
            StartCoroutine(PlayfabManager.instance.OnSignIn(5f));
            loginWithEmailButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            loadingBar.SetActive(true);
        }
        else
        {
            loginButton.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (isLoginAsGuestSuccess == true)
        {
            loadingBar.SetActive(false);
        }
    }

    public void Login(string _type)
    {
        if (_type == LoginType.Guest.ToString())
        {
            loginWithEmailButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            loadingBar.SetActive(true);
            PlayfabManager.instance.HandleLoginButtonClicked();
        }
        else if (_type == LoginType.Email.ToString())
        {
            loginPanel.SetActive(true);
            loginWithEmailButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
        }
    }

    public void AttemptEmailLogin()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(TypeText(statusText, "Please fill in all fields."));
            return;
        }

        PlayfabManager.instance.LoginWithEmail(email, password);
    }

    public void OnLoginSuccess()
    {
        loginPanel.SetActive(false);
        loadingBar.SetActive(true);
    }

    public void AttemptRegisterUser()
    {
        string username = registerUsernameInputField.text.Trim();
        string email = registerEmailInputField.text.Trim();
        string password = registerPasswordInputField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(TypeText(registerStatusText, "Please fill in all fields."));
            return;
        }

        PlayfabManager.instance.RegisterUser(username, email, password);
    }

    public void OpenForgotPasswordPanel()
    {
        forgotPasswordPanel.SetActive(true);
    }

    public void CloseForgotPasswordPanel()
    {
        forgotPasswordPanel.SetActive(false);
    }

    public void ResetPassword()
    {
        string email = forgotEmailInputField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            StartCoroutine(TypeText(forgotPasswordStatusText, "Please enter your email."));
            return;
        }

        PlayfabManager.instance.ResetPassword(email);
    }

    public void LoginTab()
    {
        registerContainer.SetActive(false);
        loginContainer.SetActive(true);
    }

    public void RegisterTab()
    {
        registerContainer.SetActive(true);
        loginContainer.SetActive(false);
    }

    public void CloseTab()
    {
        loginPanel.SetActive(false);
        loginWithEmailButton.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);
    }

    public IEnumerator TypeText(TextMeshProUGUI textComponent, string message)
    {
        textComponent.text = "";
        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
