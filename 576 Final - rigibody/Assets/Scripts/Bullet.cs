using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1000f;

    private float lifeTime = 5f;  // lift time of the projectile
    private float timer = 0f;  // record time 
    
    // Start is called before the first frame update
    void Start()
    {
        // shoot bullet
        GetComponent<Rigidbody>().AddForce(transform.right * speed);
    }

    // Update is called once per frame
    void Update()
    {
        // destroy after 5 sec of not hitting anything
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
