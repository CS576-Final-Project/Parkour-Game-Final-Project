using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Transform orientation; // orientation is set manually in unity.

    private MoveSway sway;
    private WallRun wallRun;
    private GrapplingHook playerHook;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    [Header("Movement")]
    public float walkingVelocity;
    public float crouchingVelocity;
    public float runningVelocity;
    public float wallRunningVelocity;
    public float slidingMultiplier;
    public float movementMultiplier;
    public float airMultiplier;
    public float hookMultiplier;

    [Header("Jump")]
    [SerializeField] private float jumpForce;

    [Header("Slide")]
    [SerializeField] private float slideDuration;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode walkForwardKey = KeyCode.W;
    [SerializeField] KeyCode walkLeftKey = KeyCode.A;
    [SerializeField] KeyCode walkBackwardKey = KeyCode.S;
    [SerializeField] KeyCode walkRightKey = KeyCode.D;
    [SerializeField] KeyCode crouchKey = KeyCode.C;
    [SerializeField] KeyCode runKey = KeyCode.LeftShift;

    private Vector3 moveDirection;
    private Vector3 slopeMoveDirection;

    private float rbGroundDrag = 6f;
    private float rbAirDrag = 3f;
    private float rbSlidDrag = 12f;

    private float horizontalMovement;
    private float verticalMovement;

    Rigidbody rb;

    private bool captureDirection = false;
    public bool isSliding = false;
    private Vector3 currentDirection = Vector3.zero;
    public float slideTime = 0f;

    private RaycastHit slopeHit;
    private float playerHeight = 2f;

    public bool isRopeCut = false;
    private Collider selfCollider;
    private Collider hookTriggerCollider;
    private Collider HUDTriggerCollider;
    public bool stopCapture = false;
    public Vector3 hookCurrentDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        groundDistance = 0.2f;

        walkingVelocity = 7f;
        crouchingVelocity = 3f;
        runningVelocity = 15f;
        wallRunningVelocity = 10f;
        slidingMultiplier = 0.9f; // Use multiplier because of ForceMode.VelocityChange.
        movementMultiplier = 10.5f;
        airMultiplier = 0.4f;
        hookMultiplier = 180f;

        jumpForce = 60f;

        slideDuration = 0.88f;

        sway = GameObject.Find("Head").GetComponent<MoveSway>();

        wallRun = GetComponent<WallRun>();  
        playerHook = GetComponent<GrapplingHook>();

        selfCollider = GameObject.Find("body").GetComponent<CapsuleCollider>();
    }

    void Update() {
        Inputs();
        ControlDrag();

        hookTriggerCollider = playerHook.hookHit.collider;
        if (hookTriggerCollider != null) {
            Physics.IgnoreCollision(selfCollider, hookTriggerCollider, true);
        }

        HUDTriggerCollider = playerHook.HUDHit.collider;
        if (HUDTriggerCollider != null) {
            Physics.IgnoreCollision(selfCollider, HUDTriggerCollider, true);
        }

        // Sliding and jumping can only begin on the ground.
        if (isGrounded()) {
            if (Input.GetKeyDown(jumpKey) && !isCrouchWalking() && !isCrouchStationary() && !isSliding) {
                Jump();
            }

            // Capture the initional direction at the beginning of the sliding.
            if (Input.GetKeyDown(crouchKey) && isRunning()) {
                captureDirection = true;
            }

            // Slide part.
            if ((Input.GetKeyDown(crouchKey) && isRunning()) || isSliding) {
                Slide();
                // When sliding, start the timer.
                slideTime += Time.deltaTime;
                // Duration of slide action
                if (slideTime >= slideDuration) {
                    isSliding = false;
                }
            } else if (isRunning()){
                // When sliding end, reset the time.
                slideTime = 0f;
            }

            playerHook.fired = false;
        } else if (!isGrounded()) {
            isSliding = false;
        }

        hookAcc();

        // Set slope direction.
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    // Get mouse input, set move direction.
    private void Inputs() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void ControlDrag() {
        if (isGrounded() && !isSliding) {
            rb.drag = rbGroundDrag;
        } else if (!isGrounded()){
            rb.drag = rbAirDrag;
        } else if (isGrounded() && isSliding) {
            rb.drag = rbSlidDrag;
        }
    }

    private void FixedUpdate() {
        if (isWalking()) {
            PlayerWalking();
        }

        if (isRunning() && wallRun.StopWallRun()) {
            PlayerRunning();
        } else if (isRunning() && !wallRun.StopWallRun()) {
            PlayerWallRunning();
        }

        if (isGrounded()) {
            if (isCrouchWalking()) {
                PlayerCrouchWalking();
            }
        }

        // If player is in the air and not wallrunning, use the normal gravity.
        if (!isGrounded() && wallRun.StopWallRun() && !playerHook.hooked) {
            rb.useGravity = true;
        }
    }

    private void PlayerWallRunning() {
        if (wallRun.isWallLeft) {
            rb.AddForce(orientation.forward * wallRunningVelocity * movementMultiplier - orientation.right * 10f, ForceMode.Acceleration);
        } else if (wallRun.isWallRight) {
            rb.AddForce(orientation.forward * wallRunningVelocity * movementMultiplier + orientation.right * 10f, ForceMode.Acceleration);
        } 
    }

    private void PlayerWalking() {
        if (isGrounded() && !onSlope()) {
            rb.AddForce(moveDirection.normalized * walkingVelocity * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded()) {
            rb.AddForce(moveDirection.normalized * walkingVelocity * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        } else if (onSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * walkingVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerRunning() {
        if (isGrounded() && !onSlope()) {
            rb.AddForce(moveDirection.normalized * runningVelocity * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded()) {
            rb.AddForce(moveDirection.normalized * runningVelocity * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        } else if (onSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * runningVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerCrouchWalking() {
        if (isGrounded()) {
            rb.AddForce(moveDirection.normalized * crouchingVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    public bool isGrounded() {  
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public bool onSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f)) {
            if (slopeHit.normal != Vector3.up) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    private void Jump() {
        if(isGrounded()) {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Slide() {
        if(captureDirection) {
            currentDirection = moveDirection;
        }
        if(sway.canSlide) {
            isSliding = true;
        }
        captureDirection = false;
        rb.AddForce(currentDirection.normalized * slidingMultiplier, ForceMode.VelocityChange);
    }

    // Stand part.
    public bool isWalking() {
        if ((Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey) || Input.GetKey(walkBackwardKey)) && !Input.GetKey(runKey) && !Input.GetKey(crouchKey) && !wallRun.isWallLeft && !wallRun.isWallRight) {
            return true;
        }
        return false;
    }

    public bool isRunning() {
        if ((Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey)) && Input.GetKey(runKey) && !isSliding) {
            return true;
        }
        return false;
    }

    public bool isStationary() {
        if (!isWalking() && !isRunning() && !isCrouchWalking()) {
            return true;
        }
        return false;
    }

    // Crouch part.
    public bool isCrouchStationary() {
        if (Input.GetKey(crouchKey) && isStationary() && !isRunning() && !isSliding && !onSlope()) {
            return true;
        }
        return false;
    }

    public bool isCrouchWalking() {
        if (Input.GetKey(crouchKey) && (Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey) || Input.GetKey(walkBackwardKey)) && !isRunning() && !isSliding && !onSlope()) {
            return true;
        }
        return false;
    }
    
    // Wall running part.
    public bool isWallRunningStationary() {
        if (isStationary() && !wallRun.StopWallRun()) {
            return true;
        } else {
            return false;
        }
    }

    public void hookAcc() {
        if (isRopeCut) {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            rb.AddForce(orientation.forward * hookMultiplier, ForceMode.Impulse);
        }
        isRopeCut = false;
    }
}
