using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        // shoot bullet
        GetComponent<Rigidbody>().AddForce(transform.right * speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
