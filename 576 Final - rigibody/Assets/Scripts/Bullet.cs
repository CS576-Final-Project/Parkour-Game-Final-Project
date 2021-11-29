using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 shootingDirection;

    public float speed = 1000f;  // projectile speed
    public GameObject explosionEffect;  // projectile explosion effect
    // public AudioSource inFlightAudioSource;  // audio of the projectile when in flight
    public ParticleSystem inFlightEffect;  // projectile in flight effect

    private float lifeTime = 5f;  // lift time of the projectile
    private float timer = 0f;  // record time 
    
    // Start is called before the first frame update
    void Start()
    {
        // shoot bullet
        // GetComponent<Rigidbody>().AddForce(transform.right * speed);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().AddForce(shootingDirection * speed);
        // update timer
        timer += Time.deltaTime;
        // destroy after 5 sec of not hitting anything
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
        if (!enabled) return;
        
        // explode when hitting an object
        Explode();
        // stop in flight audio
        // inFlightAudioSource.Stop();
        foreach(Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
        inFlightEffect.Stop();
        
        //  Destroy the projectile after 2 seconds. Using a delay because the particle system needs to finish
        Destroy(gameObject, 5f);
    }
    
    private void Explode()
    {
        // explosion
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation, null);
    }
    
}
