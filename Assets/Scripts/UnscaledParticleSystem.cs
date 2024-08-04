using UnityEngine;

public class UnscaledParticleSystem : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float previousTime;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        previousTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - previousTime;
            previousTime = currentTime;

            particleSystem.Simulate(deltaTime, true, false);
        }
        else{
             particleSystem.Play();
        }
    }
}
