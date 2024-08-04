using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MissionObjective : MonoBehaviour
{
    public float timer = 35f;
    public string Objectives;
    public TextMeshProUGUI objectiveText;

    public UnityEvent onStart;
    public UnityEvent onComplete;
    

    private void Start()
    {
        StartCoroutine(ShowText(timer));
        onStart?.Invoke();
    }

    public void TaskComplete()
    {
        onComplete?.Invoke();
    }

    IEnumerator ShowText(float time)
    {
       StartCoroutine(TypeSentence(Objectives));

        yield return new WaitForSeconds(time);
        objectiveText.text = "";
    }
     IEnumerator TypeSentence(string sentence)
    {
        objectiveText.text ="";
        foreach (char letter in Objectives.ToCharArray()){
            objectiveText.text += letter;
            yield return null;
        }
    }
}
