using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class TestBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        HealthBarHandler.SetHealthBarValue(1);
    }
 
    // Update is called once per frame
    void Update()
    {
        HealthBarHandler.SetHealthBarValue(HealthBarHandler.GetHealthBarValue() - 0.01f);
    }
}