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
        size = (Camera.main.transform.position - transform.position).magnitude;
        size *= 0.05f;
        transform.localScale = new Vector3(size,size,size);
    }
}
