using UnityEngine;
using TMPro;

[System.Serializable]
public class WeaponDetails
{
    public string name;
    public GameObject weapon;
    public int price;
    public string alias; // Single alias field
}

public class WeaponSelector : MonoBehaviour
{
    public GameObject weaponParent;

    public WeaponDetails[] weapons;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI aliasText; // Added TextMeshProUGUI for alias
    public TextMeshProUGUI currencyText; // Added TextMeshProUGUI for currency display
    public GameObject purchaseButton;
    public GameObject selectButton;

    private int currentIndex = 0;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f;

    private bool[] purchasedWeapons;

    // Rotation speeds for each axis
    private float rotationSpeedX = 0f;
    private float rotationSpeedY = 75f;
    private float rotationSpeedZ = 0f;

    void Start()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogError("No weapons assigned in the inspector.");
            return;
        }

        // Disable all weapons
        DisableAllWeapons();

        // Load purchased weapons from PlayerPrefs
        LoadPurchasedWeapons();

        // Ensure the first weapon is always unlocked
        purchasedWeapons[0] = true;

        UpdateCurrencyDisplay(CurrencyManager.Instance.CurrentCurrency);
        UpdatePurchaseUI();

        // Enable the first weapon and update UI texts
        EnableWeapon(0);
        UpdateUI(0);

        // Subscribe to currency changes
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
    }

    void OnDestroy()
    {
        // Unsubscribe from currency changes
        CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }

    void Update()
    {
        HandleSwipe();
    }

    public void OnRightButton()
    {
        currentIndex = (currentIndex + 1) % weapons.Length;
        UpdateWeapon();
        // SaveSelectedWeapon(); // Save the selected weapon index
    }

    public void OnLeftButton()
    {
        currentIndex = (currentIndex - 1 + weapons.Length) % weapons.Length;
        UpdateWeapon();
        // SaveSelectedWeapon(); // Save the selected weapon index
    }

    void UpdateWeapon()
    {
        DisableAllWeapons();

        // Enable the current weapon and update UI texts
        EnableWeapon(currentIndex);
        UpdateUI(currentIndex);
        UpdatePurchaseUI();
    }

    void DisableAllWeapons()
    {
        // Disable all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].weapon != null)
            {
                weapons[i].weapon.SetActive(false);
            }
        }
    }

    void EnableWeapon(int index)
    {
        if (weapons[index].weapon != null)
        {
            weapons[index].weapon.SetActive(true);
        }
    }

    void UpdateUI(int index)
    {
        WeaponDetails currentWeapon = weapons[index];
        if (nameText != null)
        {
            nameText.text = currentWeapon.name;
        }
        else
        {
            Debug.LogError("Name TextMeshProUGUI is not assigned.");
        }

        if (priceText != null)
        {
            priceText.text = $"{currentWeapon.price}";
        }
        else
        {
            Debug.LogError("Price TextMeshProUGUI is not assigned.");
        }

        if (aliasText != null)
        {
            aliasText.text = currentWeapon.alias;
        }
        else
        {
            Debug.LogError("Alias TextMeshProUGUI is not assigned.");
        }
    }

    void UpdatePurchaseUI()
    {
        bool isPurchased = purchasedWeapons[currentIndex];
        purchaseButton.SetActive(!isPurchased);
        selectButton.SetActive(isPurchased);
    }

    void UpdateCurrencyDisplay(int newAmount)
    {
        if (currencyText != null)
        {
            currencyText.text = $"{newAmount}";
        }
    }

    private void LoadPurchasedWeapons()
    {
        purchasedWeapons = new bool[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            // Load the purchase status from PlayerPrefs
            purchasedWeapons[i] = PlayerPrefs.GetInt($"WeaponPurchased_{i}", i == 0 ? 1 : 0) == 1;
        }
    }

    private void SavePurchasedWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            PlayerPrefs.SetInt($"WeaponPurchased_{i}", purchasedWeapons[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void SaveSelectedWeapon()
    {
        PlayerPrefs.SetInt("SelectedWeaponIndex", currentIndex);
        PlayerPrefs.Save();
        Debug.Log($"Saved selected weapon index: {currentIndex}");
    }

    public void OnPurchaseButton()
    {
        WeaponDetails currentWeapon = weapons[currentIndex];
        if (CurrencyManager.Instance.TryPurchase(currentWeapon.price))
        {
            purchasedWeapons[currentIndex] = true;
            SavePurchasedWeapons(); // Save the purchased status

            // Update currency display after successful purchase
            UpdateCurrencyDisplay(CurrencyManager.Instance.CurrentCurrency);

            Debug.Log($"Purchased weapon: {currentWeapon.name}");
            UpdatePurchaseUI();
        }
        else
        {
            Debug.Log("Not enough currency to purchase this weapon.");
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
                    OnLeftButton();
                }
                else
                {
                    OnRightButton();
                }
            }
        }
    }

    void RotateWeapon(GameObject _weaponParent)
    {
        // Calculate rotation for each frame
        float rotationX = rotationSpeedX * Time.deltaTime;
        float rotationY = rotationSpeedY * Time.deltaTime;
        float rotationZ = rotationSpeedZ * Time.deltaTime;

        // Apply rotation to the object
        _weaponParent.transform.Rotate(rotationX, rotationY, rotationZ);
    }
}
