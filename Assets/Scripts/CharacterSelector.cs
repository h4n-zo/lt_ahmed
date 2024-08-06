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
    public Character[] characters;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI aliasText; // Added TextMeshProUGUI for alias
    private int currentIndex = 0;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f;

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

        // Enable the first character and their extra items, and update UI texts
        EnableCharacter(0);
        UpdateUI(0);
    }

    void Update()
    {
        HandleSwipe();
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
                    OnRightButton();
                }
                else
                {
                    OnLeftButton();
                }
            }
        }
    }
}