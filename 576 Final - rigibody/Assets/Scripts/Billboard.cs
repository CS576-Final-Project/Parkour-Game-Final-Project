using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;
    
    // force the health bar to face the camera
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
