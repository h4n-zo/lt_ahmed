using UnityEngine;

public class WeaponActivator : MonoBehaviour
{
    public WeaponDetails[] weapons;

    void Start()
    {
        LoadAndActivateWeapon();
    }

    void LoadAndActivateWeapon()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedWeaponIndex", 0);
        
        if (savedIndex >= 0 && savedIndex < weapons.Length)
        {
            // Disable all weapons first
            DisableAllWeapons();

            // Enable the saved weapon
            EnableWeapon(savedIndex);

            Debug.Log($"Loaded and activated weapon at index: {savedIndex}");
        }
        else
        {
            Debug.LogWarning("No valid saved weapon index found.");
        }
    }

    void DisableAllWeapons()
    {
        foreach (WeaponDetails weapon in weapons)
        {
            if (weapon.weapon != null)
            {
                weapon.weapon.SetActive(false);
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
}
