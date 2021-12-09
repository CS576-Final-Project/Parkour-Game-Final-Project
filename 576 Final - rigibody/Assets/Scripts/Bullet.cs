using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 shootingDirection;  // shooting direction
    public float speed;  // projectile speed
    public GameObject explosionEffect;  // projectile explosion effect
    public GameObject rocketFire;
    public AudioSource inFlightAudioSource;  // audio of the projectile when in flight
    public ParticleSystem inFlightEffect;  // projectile in flight effect
    public float damage = 20;

    private float lifeTime = 3.5f;  // lift time of the projectile
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
        // update timer
        timer += Time.deltaTime;
        // destroy after 5 sec of not hitting anything
        if (timer > lifeTime)
        {
            GameObject parent = transform.parent.gameObject;
            Destroy(parent, 3f);
        }

        // shoot bullet
        transform.position += shootingDirection * speed * Time.deltaTime;
        rocketFire.transform.position += shootingDirection * speed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddForce(shootingDirection * speed, ForceMode.Acceleration);
    }

    public void OnCollisionEnter(Collision other)
    {
        // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
        if (!enabled) return;
        
        // explode when hitting an object
        Explode();
        // stop in flight audio
        inFlightAudioSource.Stop();
        foreach(Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
        inFlightEffect.Stop();
        
        // if the bullet fit player, decrease player's health
        if (other.gameObject.layer == 14)
        {
            GameObject.FindWithTag("Player").transform.GetChild(0).gameObject.GetComponent<PlayerHealth>().health -= damage;
        }
        
        //  Destroy the projectile after 2 seconds. Using a delay because the particle system needs to finish
        GameObject parent = transform.parent.gameObject;
        Destroy(parent, 3f);
        Destroy(rocketFire, 3f);
    }
    
    private void Explode()
    {
        // explosion
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation, null);
    }
    
}