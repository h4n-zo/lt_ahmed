using UnityEngine;
using UnityEngine.Events;

public class CharacterSelector : MonoBehaviour
{
    public GameObject[] characters; // Array of character GameObjects
    private int currentIndex = 0; // Current character index

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance to be considered a swipe

    void Start()
    {
        UpdateCharacter();
    }

    void Update()
    {
        HandleSwipe();
    }

    public void OnRightButton()
    {
        currentIndex++;
        if (currentIndex >= characters.Length)
        {
            currentIndex = 0;
        }
        UpdateCharacter();
    }

    public void OnLeftButton()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characters.Length - 1;
        }
        UpdateCharacter();
    }

    void UpdateCharacter()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == currentIndex);
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
