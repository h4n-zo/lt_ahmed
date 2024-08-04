using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField]
    float destroyTime = 1f;
    void Start()
    {
        Destroy(this.gameObject, destroyTime);
    }

}
