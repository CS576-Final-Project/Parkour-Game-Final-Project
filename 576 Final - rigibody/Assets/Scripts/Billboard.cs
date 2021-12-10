using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    void Start() 
    {
        cam = GameObject.FindWithTag("MainCamera").transform;
    }
    
    // force the health bar to face the camera
    void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.forward);
    }
}
