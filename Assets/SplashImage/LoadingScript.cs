using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    // private float time;
    public float second = 11f;
    // public string mainMenu;
    // public Image fillImage;



    // Start is called before the first frame update
    void Start()
    {
        // second = 5;
        Invoke("LoadGame", second);
    }

    

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
