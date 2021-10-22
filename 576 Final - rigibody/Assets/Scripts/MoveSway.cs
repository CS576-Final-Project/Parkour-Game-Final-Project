using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSway : MonoBehaviour
{
    public Transform orientation;
    public float wallDistance;

    public float swingCycle;

    private PlayerMove playerMovement;
    private WallRun wallRun;
    private float initionalLocalY;
    private float theta;
    private float smoothAmount;
    private Vector3 initionalWalkingPosition;
    private Vector3 initionalCrouchingPosition;
    private Vector3 slidingPosition;

    public Quaternion initionalRotation;
    private Vector3 middleRotationVector = new Vector3(0f, 0f, -13f);
    private Quaternion middleRotation;
    private Vector3 finalRotationVector = new Vector3(0f, 0f, 5f);
    private Quaternion finalRotation;
    private bool goBack = false;
    public bool canSlide = true;

    private Vector3 wallRunLeftRotationVector = new Vector3(0f, 0f, -20f);
    private Vector3 wallRunRightRotationVector = new Vector3(0f, 0f, 20f);
    private Quaternion wallRunLeftRotation;
    private Quaternion wallRunRightRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        wallRun = GameObject.FindWithTag("Player").GetComponent<WallRun>();
        initionalLocalY = transform.localPosition.y;
        initionalWalkingPosition = new Vector3(0f, initionalLocalY + 0.0001f, 0f);
        initionalCrouchingPosition = new Vector3(0f, 0.55f * initionalLocalY + 0.0001f, 0f);
        slidingPosition = new Vector3(0f, 0.15f * initionalLocalY + 0.0001f, 0f);
        initionalRotation = transform.localRotation;
        middleRotation = Quaternion.Euler(middleRotationVector);
        finalRotation = Quaternion.Euler(finalRotationVector);
        wallRunLeftRotation = Quaternion.Euler(wallRunLeftRotationVector);
        wallRunRightRotation = Quaternion.Euler(wallRunRightRotationVector);
        theta = 0f;
        smoothAmount = 6f;
        swingCycle = 4f;
        wallDistance = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // When player remain stationary, reset theta to 0.
        if (playerMovement.isStationary() || playerMovement.isCrouchStationary()) {
            theta = 0f;
        }

        // Control the height of the player according to the pose.
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

        if (transform.localRotation == initionalRotation) {
            canSlide = true;
        }

        if(!isWallAhead()) {
            // If player is walking, then begin sway.
            if (playerMovement.isWalking() && playerMovement.isGrounded() && !playerMovement.isCrouchWalking() && !playerMovement.isSliding) {
                //Limits on swing range. Cannot exceed the initional local Y.
                transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

                theta += 0.003f;
                this.transform.Translate(this.transform.up * Mathf.Sin(swingCycle * 2f * theta) * 0.003f);
            }

            // If player is running, then begin sway.
            if (playerMovement.isRunning() && playerMovement.isGrounded() && !playerMovement.isSliding) {
                //Limits on swing range. Cannot exceed the initional local Y.
                transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

                theta += 0.003f;
                this.transform.Translate(this.transform.up * Mathf.Sin(swingCycle * 3f * theta) * 0.006f);
            }

            // If player is crouch walking, then begin sway.
            if (playerMovement.isCrouchWalking() && playerMovement.isGrounded() && !playerMovement.isSliding) {
                theta += 0.002f;
                this.transform.Translate(this.transform.up * Mathf.Sin(swingCycle * 2f * theta) * 0.001f);
            }
        }

        // If player is sliding, set camera offset.
        if (playerMovement.isSliding) {
            canSlide = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, slidingPosition, Time.deltaTime * 4f);
            if (!goBack) {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, middleRotation, Time.deltaTime * 8f);
            } else {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, finalRotation, Time.deltaTime * 10f);
            }

            if (transform.localRotation.eulerAngles.z <= (360.5 + middleRotationVector.z)) {
                goBack = true;
            }
        }

        // If player is wall running, set the camera offset.
        if (wallRun.isWallLeft) {
            transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

            theta += 0.003f;
            this.transform.Translate(this.transform.up * Mathf.Sin(swingCycle * 2.6f * theta) * 0.006f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunLeftRotation, Time.deltaTime * 8f);
        } else if (wallRun.isWallRight) {
            transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initionalLocalY), 0f);

            theta += 0.003f;
            this.transform.Translate(this.transform.up * Mathf.Sin(swingCycle * 2.6f * theta) * 0.006f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, wallRunRightRotation, Time.deltaTime * 8f);
        }
    }

    private bool isWallAhead() {
        return Physics.Raycast(transform.position, orientation.forward, wallDistance);
    }
}
