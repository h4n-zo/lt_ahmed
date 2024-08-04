using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 3f;
    public Vector3 Offset = new Vector3(0, 3, 0);
    public Vector3 randomizeIntensity = new Vector3(0.8f, 0, 0);
    // Start is called before the first frame update
    void Start()
    {

        Destroy(gameObject, destroyTime);

        transform.localPosition = Offset;  
        // transform.localPosition = new Vector3(Random.Range(randomizeIntensity.x, randomizeIntensity.x), 
        // Random.Range(randomizeIntensity.y, randomizeIntensity.y),
        // Random.Range(randomizeIntensity.z, randomizeIntensity.z));


        
    }

    
}
