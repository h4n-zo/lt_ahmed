using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // For handling scene changes

public class CurrencyManager : MonoBehaviour
{
    private const string CURRENCY_KEY = "PlayerCurrency";

    public static CurrencyManager Instance { get; private set; }

    public int CurrentCurrency { get; private set; }

    public delegate void CurrencyChangedDelegate(int newAmount);
    public event CurrencyChangedDelegate OnCurrencyChanged;

    [SerializeField] private TextMeshProUGUI currencyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }

        ReassignUIElements();
        UpdateCurrencyDisplay(CurrentCurrency);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Register to sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unregister to avoid memory leaks
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReassignUIElements(); // Reassign UI elements after scene load
        UpdateCurrencyDisplay(CurrentCurrency);
    }

    private void ReassignUIElements()
    {
        // Reassign the TextMeshProUGUI object, if it has been lost
        if (SceneManager.GetActiveScene().name == "Game_Menu")
        {


            if (currencyText == null)
            {
                currencyText = GameObject.Find("currencyText").GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void LoadCurrency()
    {
        CurrentCurrency = PlayerPrefs.GetInt(CURRENCY_KEY, 3000); // Default starting currency
    }

    public bool TryPurchase(int amount)
    {
        if (CurrentCurrency >= amount)
        {
            CurrentCurrency -= amount;
            PlayerPrefs.SetInt(CURRENCY_KEY, CurrentCurrency);
            PlayerPrefs.Save();
            OnCurrencyChanged?.Invoke(CurrentCurrency);
            UpdateCurrencyDisplay(CurrentCurrency);
            return true;
        }
        return false;
    }

    public void AddCurrency(int amount)
    {
        CurrentCurrency += amount;
        PlayerPrefs.SetInt(CURRENCY_KEY, CurrentCurrency);
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke(CurrentCurrency);
        UpdateCurrencyDisplay(CurrentCurrency);
    }

    private void UpdateCurrencyDisplay(int newAmount)
    {
        if (currencyText != null)
        {
            currencyText.text = $"{newAmount}";
        }
    }
}
