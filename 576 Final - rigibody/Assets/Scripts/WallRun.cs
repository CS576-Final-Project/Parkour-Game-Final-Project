using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public Transform orientation;
    private PlayerMove playerMovement;

    [Header("Wall Running")]
    [SerializeField] private float wallDistance;
    [SerializeField] private float minimumJumpHeight;
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce; 

    public bool isWallLeft = false;
    public bool isWallRight = false;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        wallDistance = 0.8f;
        minimumJumpHeight = 1.5f;
        wallRunGravity = 0.1f;
        wallRunJumpForce = 18f;

        playerMovement = GetComponent<PlayerMove>();
        
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();

        if (CanWallRun()) {
            if(isWallLeft || isWallRight) {
                StartWallRun();
            }
        }
    }

    private void CheckWall() {
        isWallLeft = (Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance) && !playerMovement.isGrounded());
        isWallRight = (Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance) && !playerMovement.isGrounded());
    }

    private bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void StartWallRun() {
        if (playerMovement.isWallRunningStationary()) {
            rb.AddForce(Vector3.down * wallRunGravity * 15f, ForceMode.Force);
        } else {
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        }
        
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
