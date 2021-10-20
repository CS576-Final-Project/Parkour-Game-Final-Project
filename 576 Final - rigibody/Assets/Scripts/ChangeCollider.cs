using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCollider : MonoBehaviour
{   
    private PlayerMove player_movement;
    private CapsuleCollider player_collider;

    private Vector3 crouch_height;
    private Vector3 slide_height;

    // Start is called before the first frame update
    void Start()
    {
        player_movement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        player_collider = GetComponent<CapsuleCollider>();

        crouch_height = new Vector3(0f,-0.2f,0f);
        slide_height = new Vector3(0f,-0.3f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player_movement.isCrouchWalking() || player_movement.isCrouchStationary()) {
            player_collider.center = crouch_height;
            player_collider.radius = 0.7f;
            player_collider.height = 1.6f;
        } else if(player_movement.isSliding) {
            player_collider.center = slide_height;
            player_collider.radius = 0.6f;
            player_collider.height = 1.2f;
        } else {
            player_collider.center = Vector3.zero;
            player_collider.radius = 0.6f;
            player_collider.height = 2f;
        }
    }
}
