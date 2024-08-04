using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RampageAbility : MonoBehaviour
{

    public GameObject stealthAura;
    public AudioSource heartbeatAudio;
    public Image stealthBar;
    public TextMeshProUGUI rampageText;
    public Button rampageButton;

    public float fillSpeed = 0.5f; // Speed at which the bar fills
    public float decreaseFillSpeed = 0.1f;
    private float currentFill = 0f;
    private bool isRampageActive = false;
    private bool isFilling = true;

    public float modSpeed;
    public float modChaseSpeed;
    public float modAcceleration;
    public float modAnimatorSpeed;


    public GameObject[] Enemies;
    public bool isChecked = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the bar and text start at 0
        rampageButton.enabled = false;
        stealthBar.fillAmount = 0f;
        rampageText.text = "0";
        StartCoroutine(FillStealthBar());
    }

    // Coroutine to fill the stealth bar and update text
    IEnumerator FillStealthBar()
    {
        while (isFilling && currentFill < 1f)
        {
            currentFill += fillSpeed * Time.deltaTime;
            stealthBar.fillAmount = currentFill;
            rampageText.text = Mathf.Clamp((currentFill * 100f), 0, 100).ToString("0");
            yield return null;
        }
        // Ensure bar is fully filled and text is 100 when done
        if (currentFill >= 1f)
        {
            rampageButton.enabled = true;
            currentFill = 1f;
            stealthBar.fillAmount = 1f;
            rampageText.text = "100";
        }
    }

    // Coroutine to decrease the stealth bar and update text
    IEnumerator DecreaseStealthBar()
    {
        while (!isFilling && currentFill > 0f)
        {
            currentFill -= decreaseFillSpeed * Time.deltaTime;
            stealthBar.fillAmount = currentFill;
            rampageText.text = Mathf.Clamp((currentFill * 100f), 0, 100).ToString("0");
            yield return null;
        }
        // Ensure bar is empty and text is 0 when done
        if (currentFill <= 0f)
        {
            isChecked = false;

            heartbeatAudio.Stop();
            currentFill = 0f;
            stealthBar.fillAmount = 0f;
            rampageText.text = "0";
            stealthAura.SetActive(false);
            isRampageActive = false; // Reset the stealth active flag
            isFilling = true; // Restart filling after the stealth ability is used
            StartCoroutine(FillStealthBar());
        }
    }

    public void ExecuteAbility()
    {

        heartbeatAudio.Play();

        if (currentFill >= 1f && !isRampageActive)
        {
            rampageButton.enabled = false;
            Enemies = GameObject.FindGameObjectsWithTag("Enemy");

            isChecked = true;

            isRampageActive = true;
            stealthAura.SetActive(true);
            isFilling = false;
            StartCoroutine(DecreaseStealthBar());
        }
        else
        {
            rampageButton.enabled = false;
        }
    }

    void RemoveEnemyEffect()
    {
        foreach (var enemy in Enemies)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.speed = enemy.GetComponent<NPCController>().initialPatrolSpeed;
            agent.acceleration = enemy.GetComponent<NPCController>().initialAcceleration;
            enemy.GetComponent<NPCController>().chaseSpeed = enemy.GetComponent<NPCController>().initialChaseSpeed;
            enemy.GetComponent<Animator>().speed = 1f;
        }
    }

    void AddEnemyEffect()
    {
        
        foreach (var enemy in Enemies)
        {
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            agent.speed = modSpeed;
            agent.acceleration = modAcceleration;
            enemy.GetComponent<NPCController>().chaseSpeed = modChaseSpeed;
            enemy.GetComponent<Animator>().speed = modAnimatorSpeed;
        }
    }

    private void Update()
    {
        if (isChecked == true)
        {
            AddEnemyEffect();
        }
        else
        {
            RemoveEnemyEffect();
        }

    }
}
