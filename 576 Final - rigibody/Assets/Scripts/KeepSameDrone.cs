using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepSameDrone : MonoBehaviour
{
    private float size;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        size = (Camera.main.transform.position - transform.position).magnitude;
        size *= 0.015f;
        if (size <= 0.5f) size = 0.5f;
        if (size >= 1.2f) size = 1.2f;
        transform.localScale = new Vector3(size,size,size);
    }
}
