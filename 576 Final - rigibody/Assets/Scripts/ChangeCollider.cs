using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCollider : MonoBehaviour
{   
    private PlayerMove playerMovement;
    private CapsuleCollider playerCollider;

    private Vector3 crouchHeight;
    private Vector3 slideHeight;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        playerCollider = GetComponent<CapsuleCollider>();

        crouchHeight = new Vector3(0f, -0.2f, 0f);
        slideHeight = new Vector3(0f, -0.3f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isCrouchWalking() || playerMovement.isCrouchStationary()) {
            // Collider when crouching.
            playerCollider.center = crouchHeight;
            playerCollider.radius = 0.7f;
            playerCollider.height = 1.7f;
        } else if (playerMovement.isSliding) {
            // Collider when sliding.
            playerCollider.center = slideHeight;
            playerCollider.radius = 0.6f;
            playerCollider.height = 1.2f;
        } else {
            // Collider when normal.
            playerCollider.center = Vector3.zero;
            playerCollider.radius = 0.6f;
            playerCollider.height = 2f;
        }
    }
}
