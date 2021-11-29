using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 100f;
    public Animator animationController;

    void Start() {
        animationController = GetComponent<Animator>();
    }

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
        GetComponent<EnemyRifleman>().die = true;
        animationController.SetBool("Die", true);
        //Destroy(gameObject);
    }

    void Update() 
    {
        if (animationController.GetCurrentAnimatorStateInfo(0).IsName("Die")) {
            animationController.SetBool("NoDieLoop", false);
            if (animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f) {
                animationController.speed = 0;
            }
        }
    }
}
