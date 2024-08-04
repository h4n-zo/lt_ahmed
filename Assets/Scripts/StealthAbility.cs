using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class StealthAbility : MonoBehaviour
{
    public GameObject stealthAura;
    public AudioSource heartbeatAudio;
    public Image stealthBar;
    public TextMeshProUGUI stealthText;
    public Button stealthButton;

    public float fillSpeed = 0.5f; // Speed at which the bar fills
    public float decreaseFillSpeed = 0.1f;
    private float currentFill = 0f;
    private bool isStealthActive = false;
    private bool isFilling = true;

    [SerializeField] TriggerSound[] triggerSounds;
    private float defaultTriggerSoundValue;
    public float fillPercentage = 28f;
    private float fillPercentageValue;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the bar and text start at 0
        stealthButton.enabled = false;
        stealthBar.fillAmount = 0f;
        stealthText.text = "0";
        StartCoroutine(FillStealthBar());
    }

    // Coroutine to fill the stealth bar and update text
    IEnumerator FillStealthBar()
    {
        while (isFilling && currentFill < 1f)
        {
            currentFill += fillSpeed * Time.deltaTime;
            stealthBar.fillAmount = currentFill;
            stealthText.text = Mathf.Clamp((currentFill * 100f), 0, 100).ToString("0");
            yield return null;
        }
        // Ensure bar is fully filled and text is 100 when done
        if (currentFill >= 1f)
        {
            stealthButton.enabled = true;
            currentFill = 1f;
            stealthBar.fillAmount = 1f;
            stealthText.text = "100";
        }
    }

    // Coroutine to decrease the stealth bar and update text
    IEnumerator DecreaseStealthBar()
    {
        while (!isFilling && currentFill > 0f)
        {
            currentFill -= decreaseFillSpeed * Time.deltaTime;
            stealthBar.fillAmount = currentFill;
            stealthText.text = Mathf.Clamp((currentFill * 100f), 0, 100).ToString("0");
            yield return null;
        }
        // Ensure bar is empty and text is 0 when done
        if (currentFill <= 0f)
        {
            foreach (var ts in triggerSounds)
            {
                ts.soundRange = ts.soundRangeValue;
            }
            heartbeatAudio.Stop();
            currentFill = 0f;
            stealthBar.fillAmount = 0f;
            stealthText.text = "0";
            stealthAura.SetActive(false);
            isStealthActive = false; // Reset the stealth active flag
            isFilling = true; // Restart filling after the stealth ability is used
            StartCoroutine(FillStealthBar());
        }
    }

    public void ExecuteAbility()
    {

        heartbeatAudio.Play();

        if (currentFill >= 1f && !isStealthActive)
        {
            stealthButton.enabled = false;
            triggerSounds = FindObjectsOfType<TriggerSound>();

            defaultTriggerSoundValue = triggerSounds[0].soundRangeValue;

            fillPercentageValue = fillPercentage * (triggerSounds[0].soundRange / 100);

            foreach (var ts in triggerSounds)
            {
                ts.soundRange = ts.soundRange - fillPercentageValue;
            }

            isStealthActive = true;
            stealthAura.SetActive(true);
            isFilling = false;
            StartCoroutine(DecreaseStealthBar());
        }
    }
}
