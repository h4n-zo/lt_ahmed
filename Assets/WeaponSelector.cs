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
    private int currentIndex = 0;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f;

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

        // Enable the first weapon and update UI texts
        EnableWeapon(0);
        UpdateUI(0);
    }

    void Update()
    {
        HandleSwipe();
    }

    public void OnRightButton()
    {
        currentIndex = (currentIndex + 1) % weapons.Length;
        UpdateWeapon();
    }

    public void OnLeftButton()
    {
        currentIndex = (currentIndex - 1 + weapons.Length) % weapons.Length;
        UpdateWeapon();
    }

    void UpdateWeapon()
    {
        DisableAllWeapons();

        // Enable the current weapon and update UI texts
        EnableWeapon(currentIndex);
        UpdateUI(currentIndex);
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
