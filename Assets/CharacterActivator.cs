using UnityEngine;

public class CharacterActivator : MonoBehaviour
{
    public Character[] characters;

    void Start()
    {
        LoadAndActivateCharacter();
    }

    void LoadAndActivateCharacter()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        
        if (savedIndex >= 0 && savedIndex < characters.Length)
        {
            // Disable all characters first
            DisableAllCharacters();

            // Enable the saved character and its extra items
            EnableCharacter(savedIndex);

            Debug.Log($"Loaded and activated character at index: {savedIndex}");
        }
        else
        {
            Debug.LogWarning("No valid saved character index found.");
        }
    }

    void DisableAllCharacters()
    {
        foreach (Character character in characters)
        {
            if (character.character != null)
            {
                character.character.SetActive(false);
            }

            if (character.extraItems != null)
            {
                foreach (GameObject item in character.extraItems)
                {
                    if (item != null)
                    {
                        item.SetActive(false);
                    }
                }
            }
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
}