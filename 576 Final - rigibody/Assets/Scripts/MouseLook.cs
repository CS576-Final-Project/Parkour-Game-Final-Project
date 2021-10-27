using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;
    public Transform playerHead;
    
    private float xRotation;
    private float mouseX;
    private float mouseY;

    private GrapplingHook playerHook;

    // Start is called before the first frame update
    void Start() {
        mouseSensitivity = 10f;
        xRotation = 0f;

        // Hide the mouse cursor.
        Cursor.lockState = CursorLockMode.Locked;

        playerHook = GameObject.FindWithTag("Player").GetComponent<GrapplingHook>();
    }

    // Update is called once per frame
    void Update() {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;

        // Limits on view range of Y axis.
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        if (!playerHook.hooked) {
            // The camera rotates up and down, indicating head up and head down.
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Player game object rotates left and right, indicating the head is turned left and right.
            // playerHead is set manually in unity.
            playerHead.Rotate(Vector3.up * mouseX);
        }
    }
}