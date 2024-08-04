using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Color finalskyboxColor;
    public Color initialSkyboxColor;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = finalskyboxColor;
    }

}
