using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationOfHead : MonoBehaviour
{
    // Head is set manually in unity.
    public Transform Head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // THIS object's rotation is always same as Head's rotation.
        transform.localRotation = Head.localRotation;
    }
}
