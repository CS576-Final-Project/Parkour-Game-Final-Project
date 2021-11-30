using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private EnemyRifleman enemy;

    public float health = 100f;
    public Animator animationController;
    private GameObject player;

    void Start() {
        enemy = GetComponent<EnemyRifleman>();
        animationController = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
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
        enemy.die = true;
        enemy.diePlayerPosition = new Vector3(player.transform.position.x, enemy.gunTip.position.y, player.transform.position.z);
        animationController.SetBool("Die", true);

        //Destroy(gameObject);
    }

    void Update() 
    {
        // Death part
        if (animationController.GetCurrentAnimatorStateInfo(0).IsName("Die")) {
            animationController.SetBool("NoDieLoop", false);
            animationController.speed = 0.6f;
            if (animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f) {
                animationController.speed = 0;
                GetComponent<Rigidbody>().useGravity = false;
                Destroy(GetComponent<CapsuleCollider>());
            }
        }
    }
}
