using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI textScore;

    private int currentKill = 0;
    public int totalKills = 4;

    public float timer = 1.4f;

    public bool TaskComplete = false;

    public UnityEvent eventTaskHandler;

    public MissionObjective missionObjective;

    private void Start()
    {
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        totalKills = enemyList.Length;

        textScore.text = $"{currentKill}/{totalKills}";
    }

    public void AddScore(int _kill)
    {
        currentKill += _kill;
        textScore.text = $"{currentKill}/{totalKills}";

        if (currentKill == totalKills)
        {
            missionObjective.TaskComplete();
            // eventTaskHandler?.Invoke();
            //mission complete
            // StartCoroutine(StartCountdown(timer));
        }
    }

    private void Update()
    {
        OnComplete(TaskComplete);

    }

    public void OnComplete(bool task)
    {
        TaskComplete = task;
        
        if (TaskComplete == true)
        {
            eventTaskHandler?.Invoke();
        }

    }
  

    IEnumerator StartCountdown(float time)
    {
        yield return new WaitForSeconds(time);

    }

    public void OnStart()
    {
        StartCoroutine(StartCountdown(timer));
    }
}
