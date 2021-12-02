using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // damage to enemy
    public float damage = 25f;
    // shooting range
    public float range = 100f; 
    // impact force to the enemy
    public float impactForce = 0.3f;
    // Player Camera
    public Camera fpsCamera;
    // explosion effect
    public ParticleSystem explosionEffect; 
    // fire effect 
    public ParticleSystem fireEffect;
    // gun tip
    public Transform gunTip;
    // gun shot audio
    public AudioClip GunShotClip;
    // gun shot audio source
    public AudioSource source;
    private Vector2 audioPitch;

    // fire rate
    private float cd = 0.2f;
    // timer 
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(source != null) source.clip = GunShotClip;
        audioPitch = new Vector2(.9f, 1.1f);
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
            // fire effect at gun tip
            Instantiate(fireEffect, gunTip);

            // recoil
            GetComponent<SwitchWeaponPlace>().doRecoil = true;

            // --- Handle Audio ---
            if (source != null)
            {
                // --- Sometimes the source is not attached to the weapon for easy instantiation on quick firing weapons like machineguns, 
                // so that each shot gets its own audio source, but sometimes it's fine to use just 1 source. We don't want to instantiate 
                // the parent gameobject or the program will get stuck in a loop, so we check to see if the source is a child object ---
                if(source.transform.IsChildOf(transform))
                {
                    source.volume = 0.1f;  // control volume
                    source.Play();
                }
                else
                {
                    // Instantiate prefab for audio, delete after a few seconds
                    AudioSource newAS = Instantiate(source);
                    if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        // Change pitch to give variation to repeated shots
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
                        newAS.pitch = Random.Range(audioPitch.x, audioPitch.y);
                        
                        // Play the gunshot sound
                        newAS.volume = 0.1f;  // control volume
                        newAS.PlayOneShot(GunShotClip);

                        // Remove after a few seconds.
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }
            // shoot
            Shoot();
        }
    }

    // 
    void Shoot()
    {
        RaycastHit hit;  // store info of object hit by raycast 
        int layerMask = (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | (1 << 14);
        layerMask = ~layerMask;

        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range, layerMask))
        {
            // if we hit an enemy
            FSMRifleman enemy = hit.transform.GetComponent<FSMRifleman>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            // if we hit an target with rigidbody (eg. enemy), repel it a little bit
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse);
            }

            // explode the hit object
            Instantiate(explosionEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
