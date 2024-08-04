using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float timer = 1.4f;
    [HideInInspector] public bool isDead = false;
    public GameObject gameOverUI;
    public GameObject gameOverCam, playerCam, abilityUI, topContainer;

    public GameObject healthContainer;
    public Image healthBar; // Reference to the health bar image component
    public Color fullHealthColor; // Color for full health
    public Color midHealthColor; // Color for mid health
    public Color depletedHealthColor; // Color for depleted health

    private GameManager gameManager;



    private void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
        gameManager.minimapCanvas.SetActive(true);
        gameManager.topContainer.SetActive(true);

        healthBar.color = fullHealthColor;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        // Clamp health to ensure it's not below 0
        health = Mathf.Clamp(health, 0f, maxHealth);

        // Update health bar fill amount based on current health
        float healthPercentage = health / maxHealth;
        healthBar.fillAmount = healthPercentage;

        // Update health bar color based on current health
        if (health >= 100f)
        {
            healthBar.color = fullHealthColor;
        }
        else if (health >= 55f)
        {
            healthBar.color = midHealthColor;
        }
        else
        {
            healthBar.color = depletedHealthColor;
        }

        if (health <= 0f)
        {
            isDead = true;
            topContainer.SetActive(false);
            abilityUI.SetActive(false);
            GetComponent<PlayerScript>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Animator>().SetBool("Death", true);
            StartCoroutine(StartCountdown(timer));


        }
    }

    IEnumerator StartCountdown(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in enemies)
        {
            enemy.GetComponent<NPCController>().enabled = false;
            enemy.GetComponent<NavMeshAgent>().enabled = false;

        }
        gameManager.minimapCanvas.SetActive(false);
        gameManager.topContainer.SetActive(false);
        gameOverUI.GetComponent<Animator>().SetBool("isOpen", true);
        gameOverCam.SetActive(true);
        playerCam.SetActive(false);
        healthContainer.SetActive(false);
    }

    private void Update()
    {
        float healthPercentage = health / maxHealth;
        healthBar.fillAmount = healthPercentage;

        if (health >= 100f)
        {
            health = 100f;
            healthBar.color = fullHealthColor;
        }
        else if (health >= 55f)
        {
            healthBar.color = midHealthColor;
        }
        else
        {
            healthBar.color = depletedHealthColor;
        }
    }
}
