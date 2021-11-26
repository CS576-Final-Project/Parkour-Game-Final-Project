using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public Transform orientation; // orientation is set manually in unity.
    private PlayerMove playerMovement;
    public LayerMask wallLayer;

    [Header("Wall Running")]
    [SerializeField] private float wallDistance;
    [SerializeField] private float minimumJumpHeight;
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce; 

    // Check if the player is on wall.
    public bool isWallLeft = false;
    public bool isWallRight = false;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMove>();

        wallDistance = 1.4f;
        minimumJumpHeight = 1.5f;
        wallRunGravity = 0.1f;
        wallRunJumpForce = 15f;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();

        if (CanWallRun()) {
            if(isWallLeft || isWallRight) {
                rb.useGravity = false;
                StartWallRun();
            }
        }
    }

    private void CheckWall() {
        // The wallrun state is true only when player is in the air.
        isWallLeft = (Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, wallLayer) && !playerMovement.isGrounded());
        isWallRight = (Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, wallLayer) && !playerMovement.isGrounded());
    }

    private bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void StartWallRun() {
        // When player is stationary on the wall, then drop faster.
        if (playerMovement.isWallRunningStationary()) {
            rb.AddForce(Vector3.down * wallRunGravity * 25f, ForceMode.Force);
        } else {
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        }
        
        // Press the space to leave the wall.
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (isWallLeft) {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal * 2f;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 80f, ForceMode.Force);
            } else if (isWallRight) {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal * 2f;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 80f, ForceMode.Force);
            }
        }
    }

    public bool StopWallRun() {
        return !isWallLeft && !isWallRight;
    }
}
