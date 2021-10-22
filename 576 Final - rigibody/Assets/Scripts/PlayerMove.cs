using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    private MoveSway sway;

    public Transform orientation;

    [Header("Movement")]
    public float walkingVelocity;
    public float crouchingVelocity;
    public float runningVelocity;
    public float wallRunningVelocity;
    public float slidingMultiplier;
    public float movementMultiplier;
    public float gravity;
    [SerializeField] private float airMultiplier;

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

    private WallRun wallRun;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        groundDistance = 0.2f;

        walkingVelocity = 7f;
        crouchingVelocity = 3f;
        runningVelocity = 15f;
        wallRunningVelocity = 12f;
        slidingMultiplier = 0.8f; // Use multiplier because of ForceMode.VelocityChange.
        movementMultiplier = 8f;
        airMultiplier = 0.4f;
        gravity = 6.5f;

        jumpForce = 70f;

        slideDuration = 0.88f;

        sway = GameObject.Find("Head").GetComponent<MoveSway>();

        wallRun = GetComponent<WallRun>();
    }

    void Update() {
        Inputs();
        ControlDrag();

        print(isWallRunningStationary());
        
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
        } else if (!isGrounded()) {
            isSliding = false;
        }
        
        if (!isGrounded() && wallRun.StopWallRun()) {
            rb.AddForce(Vector3.down * gravity, ForceMode.Force);
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

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
        } else if (!isGrounded()){
            Vector3 leftAndRight= new Vector3(moveDirection.x, 0f, moveDirection.z);
            rb.AddForce(orientation.forward * walkingVelocity * movementMultiplier * airMultiplier + leftAndRight * walkingVelocity * 2f, ForceMode.Acceleration);
        } else if (onSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * walkingVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerRunning() {
        if (isGrounded() && !onSlope()) {
            rb.AddForce(moveDirection.normalized * runningVelocity * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded()){
            Vector3 leftAndRight= new Vector3(moveDirection.x, 0f, moveDirection.z);
            rb.AddForce(orientation.forward * runningVelocity * movementMultiplier * airMultiplier + leftAndRight * runningVelocity * 1.5f, ForceMode.Acceleration);
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
}
