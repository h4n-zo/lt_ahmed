using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private const string CURRENCY_KEY = "PlayerCurrency";

    public static CurrencyManager Instance { get; private set; }

    public int CurrentCurrency { get; private set; }

    public delegate void CurrencyChangedDelegate(int newAmount);
    public event CurrencyChangedDelegate OnCurrencyChanged;

    [SerializeField] TextMeshProUGUI currencyText;

    private void Awake()
    {
        // PlayerPrefs.DeleteAll();

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

        currencyText.text = CurrentCurrency.ToString();
    }


    private void LoadCurrency()
    {
        CurrentCurrency = PlayerPrefs.GetInt(CURRENCY_KEY, 10000); // Default starting currency
    }

    public bool TryPurchase(int amount)
    {
        if (CurrentCurrency >= amount)
        {
            CurrentCurrency -= amount;
            PlayerPrefs.SetInt(CURRENCY_KEY, CurrentCurrency);
            PlayerPrefs.Save();
            OnCurrencyChanged?.Invoke(CurrentCurrency);
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
    }

    void UpdateCurrencyDisplay(int newAmount)
    {
        if (currencyText != null)
        {
            currencyText.text = $"{newAmount}";
        }
    }

}