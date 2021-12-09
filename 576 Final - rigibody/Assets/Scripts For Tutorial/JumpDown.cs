using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDown : MonoBehaviour
{
    public Transform fpsCma;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        fpsCma = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - fpsCma.position);
    }
}
