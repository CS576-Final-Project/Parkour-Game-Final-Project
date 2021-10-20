using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationOfHead : MonoBehaviour
{
    public Transform Head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Head.localRotation;
    }
}
