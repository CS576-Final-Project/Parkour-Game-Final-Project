using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // This is just a test
    public Transform orientation; // orientation is set manually in unity.

    private MoveSway sway;
    private WallRun wallRun;
    private GrapplingHook playerHook;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public LayerMask wallMask;

    [Header("Movement")]
    public float walkingVelocity;
    public float crouchingVelocity;
    public float runningVelocity;
    public float wallRunningVelocity;
    public float slidingMultiplier;
    public float movementMultiplier;
    public float airMultiplier;
    public float hookMultiplier;
    public float wallHookMultiplier;

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
    private float rbSlopeSlidDrag = 2f;

    private float horizontalMovement;
    private float verticalMovement;

    public Rigidbody rb;

    private bool captureDirection = false;
    public bool isSliding = false;
    private Vector3 currentDirection = Vector3.zero;
    public float slideTime = 0f;

    private RaycastHit slopeHit;
    private float playerHeight = 2f;

    public bool isRopeCut = false;
    public bool isWallRopeCut = false;
    public Collider selfCollider;
    public Vector3 hookCurrentDirection = Vector3.zero;
    public Vector3 wallHookCurrentDirection = Vector3.zero;
    
    public float bulletTimer = 0f;
    private float bulletDuration = 2f;
    public bool isBulleting = false;
    public GameObject left;
    public GameObject right;
    public ParticleSystem speedLine;

    private bool launch = false;
    private float launchTimer = 0;

    private float ledgeGrabResumeTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        groundDistance = 0.2f;

        walkingVelocity = 7f;
        crouchingVelocity = 3f;
        runningVelocity = 12.5f;
        wallRunningVelocity = 12.5f;
        //slidingMultiplier = 1f; // Use multiplier because of ForceMode.VelocityChange.
        movementMultiplier = 10.5f;
        airMultiplier = 0.4f;
        hookMultiplier = 250f;

        //jumpForce = 60f;

        slideDuration = 0.88f;

        sway = GameObject.Find("Head").GetComponent<MoveSway>();

        wallRun = GetComponent<WallRun>();  
        playerHook = GetComponent<GrapplingHook>();

        selfCollider = GameObject.Find("body").GetComponent<CapsuleCollider>();
    }

    void Update() {
        Inputs();
        ControlDrag();

        // Ignore all the trigger collider.
        Physics.IgnoreLayerCollision(14, 9);
        Physics.IgnoreLayerCollision(14, 10);
        Physics.IgnoreLayerCollision(14, 12);
        Physics.IgnoreLayerCollision(14, 13);
        Physics.IgnoreLayerCollision(14, 18);

        // Sliding and jumping can only begin on the ground.
        if (isGrounded()) {
            if (Input.GetKeyDown(jumpKey) && !isCrouchWalking() && !isCrouchStationary() && !isSliding && !onSteepSlope()) {
                Jump();
            }

            // Capture the initional direction at the beginning of the sliding.
            if (Input.GetKeyDown(crouchKey) && isRunning()) {
                captureDirection = true;
            }

            // Slide part.
            if ((Input.GetKeyDown(crouchKey) && isRunning()) || isSliding) {
                Slide();
            }
                // When sliding, start the timer.
            //     slideTime += Time.deltaTime;
            //     // Duration of slide action
            //     if (slideTime >= slideDuration) {
            //         isSliding = false;
            //     }
            // } else if (isRunning()){
            //     // When sliding end, reset the time.
            //     slideTime = 0f;
            // }

            if (!isSliding && !onSteepSlope() && !launch) {
                speedLine.Stop();
            }
        } else if (!isGrounded()) {
            isSliding = false;
        }

        HookAcc();
        WallHookAcc();

        // Set slope direction.
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (isBulleting) {
            bulletTimer += Time.deltaTime * 4f;
            if (bulletTimer >= bulletDuration) {
                isBulleting = false;
            }
        }

        if (isBulletTimeLeft()) {
            BulletLeftMove();
        } else if (isBulletTimeRight()) {
            BulletRightMove();
        }

        if (!onSteepSlope() && launch) {
            rb.AddForce(orientation.forward * 180f, ForceMode.Impulse);
            launch = false;
            launchTimer = 0f;
        }

        if (sway.canLedgeGrab() && !isGrounded() && rb.velocity.y > -5) {
            LedgeGrab();
        }

        if (sway.doLedgeGrabRotation) {
            ledgeGrabResumeTimer += Time.deltaTime;
            if (ledgeGrabResumeTimer > 0.2f) {
                sway.doLedgeGrabRotation = false;
                ledgeGrabResumeTimer = 0f;
            }
        }

    }

    // Get mouse input, set move direction.
    private void Inputs() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    private void ControlDrag() {
        if (isGrounded() && !isSliding && !onSteepSlope()) {
            rb.drag = rbGroundDrag;
        } else if (!isGrounded()){
            rb.drag = rbAirDrag;
        } else if (isGrounded() && isSliding) {
            rb.drag = rbSlidDrag;
        } else if (isGrounded() && onSteepSlope()) {
            rb.drag = rbSlopeSlidDrag;
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

        if (onSteepSlope()) {
            launchTimer += Time.deltaTime;
            if (launchTimer > 0.8f) launch = true;
            rb.AddForce(Vector3.down * 30f, ForceMode.Acceleration);
            steepSlopeMovement();
        }

        if (isSliding) {
            slideTime += Time.deltaTime;
            // Duration of slide action
            if (slideTime >= slideDuration) {
                isSliding = false;
            }
        } else if (isRunning() || !isGrounded()){
            // When sliding end, reset the time.
            slideTime = 0f;
        }
    }

    private void PlayerWallRunning() {
        if (wallRun.isWallLeft) {
            rb.AddForce(orientation.forward * wallRunningVelocity * movementMultiplier - orientation.right * 30f, ForceMode.Acceleration);
        } else if (wallRun.isWallRight) {
            rb.AddForce(orientation.forward * wallRunningVelocity * movementMultiplier + orientation.right * 40f, ForceMode.Acceleration);
        } 
    }

    private void PlayerWalking() {
        if (isGrounded() && !onShallowSlope()) {
            rb.AddForce(moveDirection.normalized * walkingVelocity * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded()) {
            rb.AddForce(moveDirection.normalized * walkingVelocity * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        } else if (onShallowSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * walkingVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerRunning() {
        if (isGrounded() && !onShallowSlope()) {
            rb.AddForce(moveDirection.normalized * runningVelocity * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded()) {
            rb.AddForce(moveDirection.normalized * runningVelocity * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        } else if (onShallowSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * runningVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerCrouchWalking() {
        if (isGrounded()) {
            rb.AddForce(moveDirection.normalized * crouchingVelocity * movementMultiplier, ForceMode.Acceleration);
        }
    }

    public bool isGrounded() {  
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) || Physics.CheckSphere(groundCheck.position, groundDistance, wallMask);;
    }

    public bool onShallowSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.3f)) {
            if (slopeHit.normal != Vector3.up && Vector3.Angle(slopeHit.normal, Vector3.up) <= 33f) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    public bool onSteepSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.3f)) {
            if (slopeHit.normal != Vector3.up && Vector3.Angle(slopeHit.normal, Vector3.up) > 33f) {
                speedLine.Play();
                return true;
            } else {
                return false;
            }
        }
        return false;
    }

    private void steepSlopeMovement() {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        float slideVelocity = runningVelocity * movementMultiplier * Time.deltaTime * 40f;

        moveDirection = slopeDirection * -slideVelocity;
        moveDirection.y -= slopeHit.point.y;

        rb.AddForce(moveDirection.normalized * slideVelocity, ForceMode.Acceleration);
    }

    private void Jump() {
        // if(isGrounded()) {
        //     rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // }
        rb.velocity = new Vector3(moveDirection.x * 2f, moveDirection.y + 27f, moveDirection.z * 2f);
        //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Slide() {
        if(captureDirection) {
            currentDirection = moveDirection;
        }
        if(sway.canSlide) {
            isSliding = true;
            speedLine.Play();
        }
        captureDirection = false;
        rb.velocity = new Vector3(currentDirection.x * 21, currentDirection.y, currentDirection.z * 21);
    }

    // Stand part.
    public bool isWalking() {
        if ((Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey) || Input.GetKey(walkBackwardKey)) && !Input.GetKey(runKey) && !Input.GetKey(crouchKey) && !wallRun.isWallLeft && !wallRun.isWallRight && !isBulleting && !onSteepSlope()) {
            return true;
        }
        return false;
    }

    public bool isRunning() {
        if ((Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey)) && Input.GetKey(runKey) && !isSliding && !isBulleting && !onSteepSlope() || !wallRun.StopWallRun()) {
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
        if (Input.GetKey(crouchKey) && isStationary() && !isRunning() && !isSliding && !onShallowSlope()) {
            return true;
        }
        return false;
    }

    public bool isCrouchWalking() {
        if (Input.GetKey(crouchKey) && (Input.GetKey(walkForwardKey) || Input.GetKey(walkLeftKey) || Input.GetKey(walkRightKey) || Input.GetKey(walkBackwardKey)) && !isRunning() && !isSliding && !onShallowSlope()) {
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

    public bool isBulletTimeLeft() {
        if (Input.GetKey(walkLeftKey) && isBulleting && !isGrounded()) {
            return true;
        }
        return false;
    }

    public bool isBulletTimeRight() {
        if (Input.GetKey(walkRightKey) && isBulleting && !isGrounded()) {
            return true;
        }
        return false;
    }

    private void BulletLeftMove() {
         this.transform.position = Vector3.MoveTowards(this.transform.position, left.transform.position, 30 * Time.deltaTime);
    }
    
    private void BulletRightMove() {
        this.transform.position = Vector3.MoveTowards(this.transform.position, right.transform.position, 30 * Time.deltaTime);
    }

    private void HookAcc() {
        if (isRopeCut) {
            // rb.AddForce(Vector3.up * 12f, ForceMode.Impulse);
            // rb.AddForce(orientation.forward * hookMultiplier, ForceMode.Impulse);
            rb.AddForce(hookCurrentDirection.normalized * 225f, ForceMode.Impulse);
        }
        isRopeCut = false;
    }

    private void WallHookAcc() {
        if (isWallRopeCut) {
            //rb.AddForce(hookCurrentDirection.normalized * 35f, ForceMode.Impulse);
            rb.velocity = wallHookCurrentDirection.normalized * 40f;
        }
        isWallRopeCut = false;
    }

    private void LedgeGrab() {
        sway.doLedgeGrabRotation = true;
        rb.velocity = new Vector3(rb.velocity.x, 20f, rb.velocity.z);
    }
}
