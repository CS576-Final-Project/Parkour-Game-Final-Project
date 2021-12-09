using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    public bool playerDie = false;
    public HealthBar healthBar;  // player health bar

    // Start is called before the first frame update
    void Start()
    {
        healthBar.SetHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        // display current health
        healthBar.SetHealth(health);
        
        if (health <= 0) {
            playerDie = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            playerDie = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
