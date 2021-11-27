using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // damage to enemy
    public float damage = 10f;
    // shooting range
    public float range = 100f; 
    // impact force to the enemy
    public float impactForce = 30f;
    // Player Camera
    public Camera fpsCamera;
    // explosion effect
    public ParticleSystem explosionEffect; 
    // fire effect 
    public ParticleSystem fireEffect;
    // gun tip
    public Transform gunTip;
    
    // fire rate
    private float cd = 0.2f;
    // timer 
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // update timer
        timer += Time.deltaTime;
        if (timer > cd && Input.GetMouseButtonDown(0))  // fire only when gun not in cd & player press left mouse button
        {
            // reset timer
            timer = 0;
            Shoot();
        }
    }

    // 
    void Shoot()
    {
        RaycastHit hit;  // store info of object hit by raycast 
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            
            // if we hit an enemy
            Target enemy = hit.transform.GetComponent<Target>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // if we hit an target with rigidbody (eg. enemy), repel it a little bit
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            // explode the hit object
            Instantiate(explosionEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}