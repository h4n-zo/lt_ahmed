using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CustomScore : MonoBehaviour
{
    public UnityEvent onTriggerEnter;
    public UnityEvent finalTrigger;
    public GameObject[] customScore;

    public int maxValue = 0;
    public int currentValue = 0;
    public TextMeshProUGUI textScoreValue;

    private void Start()
    {
        maxValue = customScore.Length;
        textScoreValue.text = currentValue + "/" + maxValue;
    }

    public void AddScore()
    {
        currentValue++;

        textScoreValue.text = currentValue + "/" + maxValue;

    }

    private void Update()
    {
        if (currentValue >= maxValue)
        {
            finalTrigger?.Invoke();
        }
    }


}
