using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Minimalist.Bar.Quantity;

public class Collectible : MonoBehaviour
{
    [HideInInspector] public QuantityBhv healthBhv;
    public GameObject floatingText;

    public enum Collectible_Option_Type
    {
        FirstAid,
        Cash,
    }
    public Collectible_Option_Type OptionType;

    [Header("Health Value")]
    public float healthAmount = 25f;
    public MeshRenderer medkitMeshRenderer;
    public BoxCollider medkitCollider;

    [Header("Cash Value")]
    public int cashAmount = 25;

    public MeshRenderer cashMeshRenderer;
    public BoxCollider cashCollider;

    public float rotationSpeed = 100f; // Speed of rotation
    public float moveDistance = 2f;    // Distance to move up and down
    public float moveSpeed = 2f;       // Speed of the up and down movement

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
        healthBhv = GameObject.FindAnyObjectByType<QuantityBhv>();
    }

    void Update()
    {

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float move = Mathf.PingPong(Time.time * moveSpeed, moveDistance) - (moveDistance / 2f);
        transform.position = startPosition + new Vector3(0, move, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (OptionType == Collectible_Option_Type.FirstAid)
            {
                var go = Instantiate(floatingText, transform.position, Quaternion.identity);
                go.GetComponent<TextMesh>().color = Color.green;
                go.GetComponent<TextMesh>().text = "+" + healthAmount.ToString();
                // other.GetComponent<PlayerHealth>().health = healthAmount + other.GetComponent<PlayerHealth>().health;
                other.GetComponent<PlayerHealth>().UpdateHealth((int)healthAmount);

                GetComponent<Collectible>().enabled = false;
                medkitMeshRenderer.enabled = false;
                medkitCollider.enabled = false;

            }
            else if (OptionType == Collectible_Option_Type.Cash)
            {
                var go = Instantiate(floatingText, transform.position, Quaternion.identity);
                go.GetComponent<TextMesh>().text = "+" + cashAmount.ToString();
                CurrencyManager.Instance.AddCurrency(cashAmount);
                FindObjectOfType<GameManager>().totalCash += cashAmount;

                GetComponent<Collectible>().enabled = false;
                cashMeshRenderer.enabled = false;
                cashCollider.enabled = false;
            }


        }
    }


}



