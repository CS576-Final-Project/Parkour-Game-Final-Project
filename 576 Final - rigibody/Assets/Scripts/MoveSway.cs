using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSway : MonoBehaviour
{
    private PlayerMove playerMovement;

    // Wall-check part.
    public Transform orientation;
    public Transform wallForwardCheck;
    public Transform wallLeftCheck;
    public Transform wallRightCheck;
    public LayerMask wallMask;
    public float wallDistance;

    // Walking sway related parameters.
    private float smoothAmount;
    public float swingCycle;
    private float theta;

    // Camera height for the corresponding action.
    private float initionalLocalY;
    private Vector3 initionalCrouchingPosition;
    private Vector3 initionalWalkingPosition;
    private Vector3 slidingPosition;

    // Sliding camera rotation.
    private Quaternion initionalRotation;
    private Vector3 middleRotationVector = new Vector3(0f, 0f, -13f);
    private Quaternion middleRotation;
    private Vector3 finalRotationVector = new Vector3(0f, 0f, 5f);
    private Quaternion finalRotation;
    private bool slidingRotationGoBack = false; // Rightward turn must reach the maximum value before starting to turn back.
    public bool canSlide = true; // When sliding is completely completed, only then can the next slide begin.

    // Wallrunning camera rotation.
    private WallRun wallRun;
    private Vector3 wallRunLeftRotationVector = new Vector3(0f, 0f, -14f);
    private Vector3 wallRunRightRotationVector = new Vector3(0f, 0f, 14f);
    private Quaternion wallRunLeftRotation;
    private Quaternion wallRunRightRotation;

    // Bullet time camera rotation.
    private Vector3 bulletTimeLeftRotationVector = new Vector3(0f, 0f, 15f);
    private Vector3 bulletTimeRightRotationVector = new Vector3(0f, 0f, -15f);
    private Quaternion bulletTimeLeftRotation;
    private Quaternion bulletTimeRightRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        
        // Wall-check part.
        // orientation & wallCheck are set manually in unity.
        wallDistance = 0.25f;

        // Walking sway related parameters.
        smoothAmount = 6f;
        swingCycle = 4f;
        theta = 0f;

        // Camera height for the corresponding action.
        initionalLocalY = transform.localPosition.y;
        initionalCrouchingPosition = new Vector3(0f, 0.55f * initionalLocalY + 0.0001f, 0f);
        initionalWalkingPosition = new Vector3(0f, initionalLocalY + 0.0001f, 0f);
        slidingPosition = new Vector3(0f, 0.12f * initionalLocalY + 0.0001f, 0f);

        // Sliding camera rotation.
        initionalRotation = transform.localRotation;
        middleRotation = Quaternion.Euler(middleRotationVector);
        finalRotation = Quaternion.Euler(finalRotationVector);

        // Wallrunning camera rotation.
        wallRun = GameObject.FindWithTag("Player").GetComponent<WallRun>();
        wallRunLeftRotation = Quaternion.Euler(wallRunLeftRotationVector);
        wallRunRightRotation = Quaternion.Euler(wallRunRightRotationVector);

        // Bullet time camera rotation.
        bulletTimeLeftRotation = Quaternion.Euler(bulletTimeLeftRotationVector);
        bulletTimeRightRotation = Quaternion.Euler(bulletTimeRightRotationVector);
    }

    // Update is called once per frame
    void Update()
    {
        // When player remain stationary, reset theta to 0.
        if (playerMovement.isStationary() || playerMovement.isCrouchStationary()) {
            theta = 0f;
        }

        // If not sliding nor bullett moving, back to normal.
        if (!playerMovement.isSliding && !playerMovement.isBulletTimeLeft() && !playerMovement.isBulletTimeRight() && !wallRun.isWallLeft && !wallRun.isWallRight) {
            slidingRotationGoBack = false;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initionalRotation, Time.fixedDeltaTime);
        }

        // Control the height of the camera according to the pose.
        if (playerMovement.isGrounded() && !playerMovement.isSliding) {
            if (playerMovement.isCrouchStationary() || playerMovement.isCrouchWalking()) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initionalCrouchingPosition, Time.deltaTime * smoothAmount);
            } else if (playerMovement.isStationary() || playerMovement.isWalking() || playerMovement.isRunning()) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initionalWalkingPosition, Time.deltaTime * smoothAmount);
            }
        }

        // When sliding is completely completed, allow next sliding.
        if (transform.localRotation == initionalRotation) {
            canSlide = true;
        }

        // If there is a wall ahead, stop swaying.
        if(!isWallAhead()) {
            // If player is walking, then begin sway.
            if (playerMovement.isWalking() && playerMovement.isGrounded() && !playerMovement.isCrouchWalking() && !playerMovement.isSliding) {
                WalkingSway();
            }

            // If player is running, then begin sway.
            if (playerMovement.isRunning() && playerMovement.isGrounded() && !playerMovement.isSliding) {
                RunningSway();
            }

            // If player is crouch walking, then begin sway.
            if (playerMovement.isCrouchWalking() && playerMovement.isGrounded() && !playerMovement.isSliding) {
                CrouchWalkingSway();
            }
        }

        // If player is sliding, set camera offset.
        if (playerMovement.isSliding) {
            SlidingSway();
        }

        // If player is wallrunning, set the camera offset.
        if (wallRun.isWallLeft) {
            WallRunningLeftSway();
        } else if (wallRun.isWallRight) {
            WallRunningRightSway();
        }

        if (playerMovement.isBulletTimeLeft()) {
            BulletTimeLeftSway();
        } else if (playerMovement.isBulletTimeRight()) {
            BulletTimeRightSway();
        }
    }

    void FixedUpdate() {

    }

    public bool isWallAhead() {
        return Physics.CheckSphere(wallForwardCheck.position, wallDistance, wallMask);
    }

    public bool isWallLeft() {
        return Physics.CheckSphere(wallLeftCheck.position, wallDistance, wallMask);
    }

    public bool isWallRight() {
        return Physics.CheckSphere(wallRightCheck.position, wallDistance, wallMask);
    }

    private void WalkingSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 2f * theta) * 0.003f);
    }

    private void RunningSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 3f * theta) * 0.006f);
    }

    private void CrouchWalkingSway() {
        theta += 0.002f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 2f * theta) * 0.001f);
    }

    private void SlidingSway() {
        // While sliding, cannot slide again.
        canSlide = false;

        transform.localPosition = Vector3.Lerp(transform.localPosition, slidingPosition, Time.deltaTime * 4f);
        if (!slidingRotationGoBack) {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, middleRotation, Time.deltaTime * 8f);
        } else {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * 10f);
        }

        // When reaching the maximum value of the rightward turn, going back.
        if (transform.localRotation.eulerAngles.z <= (360.5 + middleRotationVector.z)) {
            slidingRotationGoBack = true;
        }
    }
    
    private void WallRunningLeftSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 3f * theta) * 0.006f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunLeftRotation, Time.deltaTime * 8f);
    }

    private void WallRunningRightSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 3f * theta) * 0.006f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunRightRotation, Time.deltaTime * 8f);
    }

    private void BulletTimeLeftSway() {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, bulletTimeLeftRotation, Time.fixedDeltaTime * 0.3f);
    }

    private void BulletTimeRightSway() {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, bulletTimeRightRotation, Time.fixedDeltaTime * 0.3f);
    }
}
