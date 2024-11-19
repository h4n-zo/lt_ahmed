using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int missionIndex;
    public GameObject minimapCanvas;
    public GameObject topContainer;
    public GameObject missionCompleteCanvas;
    public int targetFrame = 60;
    public TextMeshProUGUI fpsText;

    public int totalCash;
    float dt = 0.0f;


    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = targetFrame;
    }

    private void Update()
    {

        if (fpsText == null)
        {
            return;
        }
        else
        {
            dt += (Time.unscaledDeltaTime - dt) * 0.1f;
            fpsText.text = (1.0f / dt).ToString("F1") + "FPS";
        }
    }
    // Start is called before the first frame update
    public void Restart()
    {
        AdsManager.Instance.ShowInterstitial();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Game_Menu");
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MissionComplete()
    {
        GameObject.Find("cashAmountText").GetComponent<TextMeshProUGUI>().text = totalCash.ToString();
        missionCompleteCanvas.GetComponent<Animator>().SetBool("isOpen", true);
        AdsManager.Instance.ShowInterstitial();

    }

}
