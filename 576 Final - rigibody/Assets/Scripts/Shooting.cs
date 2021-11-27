using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // damage to enemy
    public float damage = 10f; 
    // range
    public float range = 100f; 
    // Player Camera
    public Camera fpsCamera;
    // explosion effect
    public ParticleSystem explosionEffect; 
    // gun tip
    public Transform gunTip;
    
    // cd of each fire 
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
            Debug.Log(hit.transform.name);
            // explode the hit object
            Instantiate(explosionEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
