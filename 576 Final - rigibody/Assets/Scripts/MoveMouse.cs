using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMouse : MonoBehaviour
{
    // cameraPosition is set manually in unity.
    public Transform cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // THIS object's position is always same as cameraPosition's position.
        transform.position = cameraPosition.position;
    }
}
