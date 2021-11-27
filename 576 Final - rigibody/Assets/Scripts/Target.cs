using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;

    // reflection of the target when hitting by player
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            // target die
            Die();
        }
    }
    
    // destroy the game object if it "die"
    void Die()
    {
        Destroy(gameObject);
    }
    
}
