using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 18f;
    public float shootingRange = 12.5f;

    public Transform origin;
    public ParticleSystem[] muzzleFlash;
    public GameObject impactFX;

    public bool isFiring;

    public void Shoot(Transform target)
    {
        // Calculate the direction to the target
        Vector3 direction = target.position - origin.position;
        float distance = direction.magnitude;

        // Draw a line from the weapon's origin to the target's position
        Debug.DrawLine(origin.position, target.position, Color.red, 1.0f);
        Debug.Log("Shooting distance = " + distance);
        // Play the muzzle flash effects
        foreach (var p in muzzleFlash)
        {
            p.Play();
            p.loop = true;
        }

        // Apply damage to the player
        if (distance <= shootingRange)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(damage);
        }


        // Create impact effect at the target's position
        GameObject impactGO = Instantiate(impactFX, target.position, Quaternion.LookRotation(direction));
        Destroy(impactGO, 1f);
    }

    public void StopShooting()
    {
        foreach (var p in muzzleFlash)
        {
            p.Stop();
            p.loop = false;
        }
    }

    // Method to align the weapon's z rotation with the enemy's transform
    public void AlignWithEnemy(Transform player)
    {
        Vector3 direction = player.position - origin.position;
        direction.y = 0f; // Ensure no rotation around y-axis
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 enemyDir = player.position - transform.position;
        Quaternion enemyRot = Quaternion.LookRotation(enemyDir);
        transform.rotation = enemyRot;
        origin.rotation = rotation;
    }
}
