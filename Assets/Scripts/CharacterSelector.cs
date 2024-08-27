using UnityEngine;
using TMPro;

[System.Serializable]
public class Character
{
    public string name;
    public GameObject character;
    public int price;
    public GameObject[] extraItems;
    public string alias; // Single alias field
}

public class CharacterSelector : MonoBehaviour
{
    public GameObject characterParent;

    public Character[] characters;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI aliasText; // Added TextMeshProUGUI for alias
    private int currentIndex = 0;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f;

    //Shop Variables
    public TextMeshProUGUI currencyText;
    public GameObject purchaseButton;
    public GameObject selectButton;

    private bool[] purchasedCharacters;


    // Rotation speeds for each axis
    private float rotationSpeedX = 0f;
    //private float rotationSpeedY = 49.5f;
    private float rotationSpeedY = 75f;
    private float rotationSpeedZ = 0f;

    void RotateCharacter(GameObject _characterParent)
    {
        // Calculate rotation for each frame
        float rotationX = rotationSpeedX * Time.deltaTime;
        float rotationY = rotationSpeedY * Time.deltaTime;
        float rotationZ = rotationSpeedZ * Time.deltaTime;

        // Apply rotation to the object
        _characterParent.transform.Rotate(rotationX, rotationY, rotationZ);
    }

    void Start()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogError("No characters assigned in the inspector.");
            return;
        }

        // Disable all characters and their extra items
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].character != null)
            {
                characters[i].character.SetActive(false);
            }
            else
            {
                Debug.LogError($"Character at index {i} is not assigned.");
            }

            if (characters[i].extraItems != null)
            {
                foreach (GameObject item in characters[i].extraItems)
                {
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError($"Extra item at index {i} is not assigned.");
                    }
                }
            }
        }



        // Load purchased characters from PlayerPrefs
        LoadPurchasedCharacters();

        // Ensure the first character is always unlocked
        purchasedCharacters[0] = true;

        UpdateCurrencyDisplay(CurrencyManager.Instance.CurrentCurrency);

        UpdatePurchaseUI();

        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;



        // Enable the first character and their extra items, and update UI texts
        EnableCharacter(0);
        UpdateUI(0);
    }

    void OnDestroy()
    {
        CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }

    void Update()
    {
        HandleSwipe();
        RotateCharacter(characterParent);

    }

    public void OnRightButton()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacter();
    }

    public void OnLeftButton()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacter();
    }



    void UpdateCharacter()
    {
        // Disable all characters and their extra items
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].character != null)
            {
                characters[i].character.SetActive(false);
            }

            if (characters[i].extraItems != null)
            {
                foreach (GameObject item in characters[i].extraItems)
                {
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }
            }
        }

        // Enable the current character and their extra items, and update UI texts
        EnableCharacter(currentIndex);
        UpdateUI(currentIndex);
        UpdatePurchaseUI();
    }

    void UpdatePurchaseUI()
    {
        bool isPurchased = purchasedCharacters[currentIndex];
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

    private void LoadPurchasedCharacters()
    {
        purchasedCharacters = new bool[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            // Load the purchase status from PlayerPrefs
            purchasedCharacters[i] = PlayerPrefs.GetInt($"CharacterPurchased_{i}", i == 0 ? 1 : 0) == 1;
        }
    }

    private void SavePurchasedCharacters()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            PlayerPrefs.SetInt($"CharacterPurchased_{i}", purchasedCharacters[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void OnPurchaseButton()
    {
        Character currentCharacter = characters[currentIndex];
        if (CurrencyManager.Instance.TryPurchase(currentCharacter.price))
        {
            purchasedCharacters[currentIndex] = true;
            SavePurchasedCharacters(); // Save the purchased status

            Debug.Log($"Purchased character: {currentCharacter.name}");
            UpdatePurchaseUI();
        }
        else
        {
            Debug.Log("Not enough currency to purchase this character.");
        }
    }



    void EnableCharacter(int index)
    {
        if (characters[index].character != null)
        {
            characters[index].character.SetActive(true);
        }

        if (characters[index].extraItems != null)
        {
            foreach (GameObject item in characters[index].extraItems)
            {
                if (item != null)
                {
                    item.SetActive(true);
                }
            }
        }
    }



    public void SaveSelectedCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacterIndex", currentIndex);
        PlayerPrefs.Save();
        Debug.Log($"Saved character index: {currentIndex}");
    }

    // Add this method to be called when you want to save the character
    public void OnSaveButton()
    {
        SaveSelectedCharacter();
    }

    void UpdateUI(int index)
    {
        Character currentCharacter = characters[index];
        if (nameText != null)
        {
            nameText.text = currentCharacter.name;
        }
        else
        {
            Debug.LogError("Name TextMeshProUGUI is not assigned.");
        }

        if (priceText != null)
        {
            priceText.text = $"{currentCharacter.price}";
        }
        else
        {
            Debug.LogError("Price TextMeshProUGUI is not assigned.");
        }

        if (aliasText != null)
        {
            aliasText.text = currentCharacter.alias;
        }
        else
        {
            Debug.LogError("Alias TextMeshProUGUI is not assigned.");
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
}