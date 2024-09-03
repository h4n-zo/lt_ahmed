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

    public int totalCash;


    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = targetFrame;
    }
    // Start is called before the first frame update
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
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
