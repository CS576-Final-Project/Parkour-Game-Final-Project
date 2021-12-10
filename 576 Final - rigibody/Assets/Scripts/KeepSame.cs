using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepSame : MonoBehaviour
{
    private float size;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null) {
            size = (Camera.main.transform.position - transform.position).magnitude;
            size *= 0.05f;
            if (size <= 0.5f) size = 0.5f;
            if (size >= 2.2f) size = 2.2f;
            transform.localScale = new Vector3(size,size,size);
        }
    }
}
