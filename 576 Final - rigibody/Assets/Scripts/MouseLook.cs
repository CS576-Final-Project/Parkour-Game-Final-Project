using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouse_sensitivity;
    public Transform player_head;
    public Transform camera_holder;
    
    private float x_rotation, y_rotation;
    private float mouseX;
    private float mouseY;

    // Start is called before the first frame update
    void Start() {
        player_head = GameObject.FindWithTag("Player").transform;

        mouse_sensitivity = 10f;
        x_rotation = 0f;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        mouseX = Input.GetAxis("Mouse X") * mouse_sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouse_sensitivity;

        x_rotation -= mouseY;

        //Limits on view range of Y axis.
        x_rotation = Mathf.Clamp(x_rotation, -70f, 70f);

        //The camera rotates up and down, indicating head up and head down.
        transform.localRotation = Quaternion.Euler(x_rotation, 0f, 0f);

        //Player game object rotates left and right, indicating the head is turned left and right.
        camera_holder.Rotate(Vector3.up * mouseX);

    }
}