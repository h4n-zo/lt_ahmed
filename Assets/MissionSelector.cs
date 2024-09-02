using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MissionSelector : MonoBehaviour
{
    private int currentIndex = 0;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance to be considered a swipe

    public TextMeshProUGUI missionText;
    public TextMeshProUGUI levelNumberText; // New TextMeshProUGUI variable for level number
    public List<string> missionList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        UpdateMissionList(); // Initialize mission list
    }

    // Update is called once per frame
    void Update()
    {
        HandleSwipe();
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

    public void OnRightButton()
    {
        currentIndex++;
        if (currentIndex >= missionList.Count)
        {
            currentIndex = 0;
        }
        UpdateMissionDisplay();
    }

    public void OnLeftButton()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = missionList.Count - 1;
        }
        UpdateMissionDisplay();
    }

    public void LoadCurrentMission()
    {
        if (missionList.Count > 0)
        {
            string sceneName = missionList[currentIndex];
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("No missions available to load.");
        }
    }

    // Update the mission list to include completed levels
    void UpdateMissionList()
    {
        missionList.Clear(); // Clear existing list
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            if (PlayerPrefs.GetInt("Level_" + i, 0) == 1) // Check if level is completed
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                missionList.Add(sceneName); // Add completed level to the list
            }
        }

        // Update the mission display to reflect the current index
        if (missionList.Count > 0)
        {
            UpdateMissionDisplay();
        }
        else
        {
            missionText.text = "No completed levels found.";
            levelNumberText.text = ""; // Clear the level number text
        }
    }

    void UpdateMissionDisplay()
    {
        // Update mission text and level number text based on currentIndex
        missionText.text = missionList[currentIndex];
        levelNumberText.text = "Level " + (currentIndex + 1).ToString(); // Display level number starting from 1
    }
}
