using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public bool isPaused = false;

    public GameObject pauseCam;
    public GameObject playerCamera;
    public GameObject pauseCanvas;
    public GameObject topContainer;
    public GameObject minimapCanvas;
    [SerializeField] private GameObject[] Enemies;
    public TextMeshProUGUI levelName;


    private bool wasPaused = false;
    public const string IS_OPEN = "isOpen";

    private CinemachineBrain cinemachineBrain;
    private float transitionDuration;

    void Start()
    {
        // Get the CinemachineBrain component and transition duration
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        transitionDuration = cinemachineBrain.m_DefaultBlend.m_Time;
        levelName.text = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (isPaused && !wasPaused)
        {
            StartCoroutine(StartPause(transitionDuration));
            wasPaused = true;
        }
        else if (!isPaused && wasPaused)
        {
            StartCoroutine(ResumePause(transitionDuration));
            wasPaused = false;
        }
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Pause()
    {
        isPaused = true;
    }

    IEnumerator StartPause(float t)
    {
        PlayerScript playerScript = FindObjectOfType<PlayerScript>();
        playerScript.enabled = false;
        playerScript.GetComponent<NavMeshAgent>().enabled = false;

        pauseCanvas.GetComponent<Animator>().SetBool(IS_OPEN, true);
        pauseCam.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
        topContainer.SetActive(false);
        minimapCanvas.SetActive(false);
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            enemy.GetComponent<NPCController>().enabled = false;
            enemy.GetComponent<TriggerSound>().enabled = false;
        }

        yield return new WaitForSecondsRealtime(t);

        Time.timeScale = 0;
    }

    IEnumerator ResumePause(float t)
    {
        Time.timeScale = 1f;

        PlayerScript playerScript = FindObjectOfType<PlayerScript>();
        playerScript.enabled = true;
        playerScript.GetComponent<NavMeshAgent>().enabled = true;

        pauseCanvas.GetComponent<Animator>().SetBool(IS_OPEN, false);
        pauseCam.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        minimapCanvas.SetActive(true);

        topContainer.SetActive(true);
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in Enemies)
        {
            enemy.GetComponent<NavMeshAgent>().enabled = true;
            enemy.GetComponent<NPCController>().enabled = true;
            enemy.GetComponent<TriggerSound>().enabled = true;
        }

        yield return new WaitForSecondsRealtime(t);

        // pauseCanvas.SetActive(false);
    }

     public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Sound()
    {
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
