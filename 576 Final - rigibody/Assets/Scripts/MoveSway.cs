using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSway : MonoBehaviour
{
    private PlayerMove playerMovement;

    // Wall-check part.
    public Transform orientation;
    public Transform wallCheck;
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
    private bool goBack = false; // Rightward turn must reach the maximum value before starting to turn back.
    public bool canSlide = true; // When sliding is completely completed, only then can the next slide begin.

    // Wallrunning camera rotation.
    private WallRun wallRun;
    private Vector3 wallRunLeftRotationVector = new Vector3(0f, 0f, -20f);
    private Vector3 wallRunRightRotationVector = new Vector3(0f, 0f, 20f);
    private Quaternion wallRunLeftRotation;
    private Quaternion wallRunRightRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        
        // Wall-check part.
        // orientation & wallCheck are set manually in unity.
        wallDistance = 1f;

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
    }

    // Update is called once per frame
    void Update()
    {
        // When player remain stationary, reset theta to 0.
        if (playerMovement.isStationary() || playerMovement.isCrouchStationary()) {
            theta = 0f;
        }

        // Control the height of the camera according to the pose.
        if (playerMovement.isGrounded() && !playerMovement.isSliding) {
            if (playerMovement.isCrouchStationary() || playerMovement.isCrouchWalking()) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initionalCrouchingPosition, Time.deltaTime * smoothAmount);
            } else if (playerMovement.isStationary() || playerMovement.isWalking() || playerMovement.isRunning()) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initionalWalkingPosition, Time.deltaTime * smoothAmount);
            }
        }

        // If not sliding, back to normal.
        if (!playerMovement.isSliding) {
            goBack = false;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initionalRotation, Time.deltaTime * 7f);
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

        // If player is wall running, set the camera offset.
        if (wallRun.isWallLeft) {
            WallRunningLeftSway();
        } else if (wallRun.isWallRight) {
            WallRunningRightSway();
        }
    }

    private bool isWallAhead() {
        return Physics.Raycast(wallCheck.position, orientation.forward, wallDistance);
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
        if (!goBack) {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, middleRotation, Time.deltaTime * 8f);
        } else {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * 10f);
        }

        // When reaching the maximum value of the rightward turn, going back.
        if (transform.localRotation.eulerAngles.z <= (360.5 + middleRotationVector.z)) {
            goBack = true;
        }
    }
    
    private void WallRunningLeftSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 2.6f * theta) * 0.006f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunLeftRotation, Time.deltaTime * 8f);
    }

    private void WallRunningRightSway() {
        //Limits on swing range. Cannot exceed the initional local Y.
        transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

        theta += 0.003f;
        this.transform.Translate(this.transform.up * Mathf.Cos(swingCycle * 2.6f * theta) * 0.006f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunRightRotation, Time.deltaTime * 8f);
    }
}
