using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Gun tip position
    public Transform gunTip;
    // explosion effect
    // public GameObject explosionEffect;
    public ParticleSystem explosionEffect;
    
    // time lag of fire
    private float cd = 0.3f;
    // timer
    private float timer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        // if not in cd & player press 
        if (timer > cd && Input.GetMouseButton(0))
        {
            Debug.Log("Fire!");
            // reset timer
            timer = 0;
            // explosion at gun tip
            Object effect = Instantiate(explosionEffect, gunTip.position, gunTip.rotation);
            Destroy(effect);
        }

    }
}
