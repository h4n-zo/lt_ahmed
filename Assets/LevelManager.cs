using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        CompleteLevel();
    }
    
    public void CompleteLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        MarkLevelAsCompleted(currentLevelIndex);

        Debug.Log("Mission is saved as complete for Menu System");

        // Additional logic for completing the level (e.g., show completion screen)
    }

    void MarkLevelAsCompleted(int levelIndex)
    {
        PlayerPrefs.SetInt("Level_" + levelIndex, 1); // 1 indicates completed
        PlayerPrefs.Save(); // Save changes to PlayerPrefs
        Debug.Log("Level " + levelIndex + " marked as completed."); // Debug log
    }

}
