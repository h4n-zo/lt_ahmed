using UnityEngine;
using Cinemachine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuSelect : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    public GameObject[] canvasGO;
    private int currentIndex = 0; // Current camera index

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance to be considered a swipe

    [Header("Character Select Functionality")]
    public UnityEvent _triggerSelect;
    public UnityEvent _triggerUnselect;

    [Header("Shop Select Functionality")]
    public UnityEvent _triggerShopSelect;
    public UnityEvent _triggerShopUnselect;

    void Start()
    {
        UpdateCamera();
        Time.timeScale = 1f;
    }

    void Update()
    {
        HandleSwipe();
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #region Character Select Methods
    public void TriggerCharacterSelect()
    {
        _triggerSelect?.Invoke();
    }

    public void TriggerCharacterUnselect()
    {
        _triggerUnselect?.Invoke();
    }
    #endregion

    #region Shop Select Methods
    public void TriggerShopSelect()
    {
        _triggerShopSelect?.Invoke();
    }

    public void TriggerShopUnselect()
    {
        _triggerShopUnselect?.Invoke();
    }

    #endregion




    public void OnRightButton()
    {
        currentIndex++;
        if (currentIndex >= virtualCameras.Length)
        {
            currentIndex = 0;
        }
        UpdateCamera();
    }

    public void OnLeftButton()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = virtualCameras.Length - 1;
        }
        UpdateCamera();
    }

    void UpdateCamera()
    {
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            virtualCameras[i].gameObject.SetActive(i == currentIndex);
            canvasGO[i].gameObject.SetActive(i == currentIndex);
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
                    // OnRightButton();
                    OnLeftButton();
                }
                else
                {
                    // OnLeftButton();
                    OnRightButton();
                }
            }
        }
    }
}
