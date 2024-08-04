using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public SpriteRenderer minimapIcon;
    public Color deathColor;

    [Header("Collectibles")]
    public GameObject cashCollectible;

    private GameObject preferredCollectible;

    public float tossDistance = 0.5f; // Distance to toss the collectible
    public float springDuration = 0.5f; // Duration of the springing effect
    public float rigidbodyDelay = 2f; // Delay before adding Rigidbody component

    public enum Collectible_Option
    {
        Show_Collectible,
        Hide_Collectible,
    }
    public enum Collectible_Option_Type
    {
        Cash,
    }

    public Collectible_Option collectible_Option;
    public Collectible_Option_Type collectible_Option_Type;

    private void Start()
    {

        CollectibleType();
        CollectiblePreference();

    }

    void Update()
    {
        minimapIcon.color = deathColor;
    }

    private IEnumerator SpringOutEffect(Transform collectibleTransform)
    {


        float elapsedTime = 0f;

        while (elapsedTime < springDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / springDuration;
            float springValue = Mathf.Sin(t * Mathf.PI); // Creates a springing effect
            yield return null;
        }
        // Wait for the delay before adding Rigidbody
        yield return new WaitForSeconds(rigidbodyDelay);

        // Add Rigidbody component
        Rigidbody rb = collectibleTransform.gameObject.AddComponent<Rigidbody>();

        // (Optional) Configure the Rigidbody if needed
        // rb.mass = 1f;
        // rb.linearDamping = 0.5f;
        // rb.angularDamping = 0.05f;
    }

    public void CollectibleType()
    {

        if (collectible_Option_Type == Collectible_Option_Type.Cash)
        {
            preferredCollectible = cashCollectible;
        }
    }

    public void CollectiblePreference()
    {
        if (collectible_Option == Collectible_Option.Hide_Collectible)
        {
            Debug.Log("Spawn nothing");
        }

        else if (collectible_Option == Collectible_Option.Show_Collectible)
        {

            Vector3 randomOffset = new Vector3(
               tossDistance,
               UnityEngine.Random.Range(0, tossDistance), // Ensure upward motion
               tossDistance
           );

            // Instantiate the preferredCollectible at the current position plus the random offset
            Vector3 spawnPosition = this.transform.position + randomOffset;
            GameObject collectible = Instantiate(preferredCollectible, spawnPosition, Quaternion.identity);

            // Start the spring out effect coroutine
            // StartCoroutine(SpringOutEffect(collectible.transform));
        }
    }
}
