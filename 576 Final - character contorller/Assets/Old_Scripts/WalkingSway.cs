using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSway : MonoBehaviour
{
    public float swing_cycle;

    private PlayerMovement player_movement;
    private float initional_localY;
    private float theta;
    private float smooth_amount;
    private Vector3 initional_walking_position;
    private Vector3 initional_crouching_position;
    private Vector3 sliding_position;

    public Quaternion initional_rotation;
    private Vector3 middle_rotation_vector = new Vector3(0f, 0f, -15f);
    private Quaternion middle_rotation;
    private Vector3 final_rotation_vector = new Vector3(0f, 0f, 5f);
    private Quaternion final_rotation;
    private bool go_back = false;

    // Start is called before the first frame update
    void Start()
    {
        player_movement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        initional_localY = transform.localPosition.y;
        initional_walking_position = new Vector3(0f, initional_localY + 0.0001f, 0f);
        initional_crouching_position = new Vector3(0f, 0.55f * initional_localY + 0.0001f, 0f);
        sliding_position = new Vector3(0f, 0.2f * initional_localY + 0.0001f, 0f);
        initional_rotation = transform.localRotation;
        middle_rotation = Quaternion.Euler(middle_rotation_vector);
        final_rotation = Quaternion.Euler(final_rotation_vector);
        theta = 0f;
        smooth_amount = 6f;
        swing_cycle = 4f;
    }

    // Update is called once per frame
    void Update()
    {
        // When player remain stationary, reset theta to 0.
        if(player_movement.isStationary() && player_movement.isCrouchStationary()) {
            theta = 0f;
        }

        // Control the height of the player according to the pose.
        if(player_movement.IsGrounded() && !player_movement.isSliding) {
            if(player_movement.isCrouchStationary() || player_movement.isCrouchWalking()) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initional_crouching_position, Time.deltaTime * smooth_amount);
            } else if(player_movement.isStationary() || player_movement.isWalking() || player_movement.isRunning() && !player_movement.isCrouchWalking()){
                transform.localPosition = Vector3.Lerp(transform.localPosition, initional_walking_position, Time.deltaTime * smooth_amount);
            }
        }

        // If not sliding, back to normal.
        if(!player_movement.isSliding && !player_movement.isDuringSliding()) {
            go_back = false;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initional_rotation, Time.deltaTime * 10f);
        }

        // If player is walking, then begin sway.
        if(player_movement.isWalking() && player_movement.IsGrounded() && !player_movement.isSliding) {
            //Limits on swing range. Cannot exceed the initional local Y.
            transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initional_localY), 0f);
            
            theta += 0.003f;
            this.transform.Translate(this.transform.up * Mathf.Sin(swing_cycle * 2f * theta) * 0.003f);
        } 
        
        // If player is running, then begin sway.
        if(player_movement.isRunning() && player_movement.IsGrounded() && !player_movement.isSliding) {
            //Limits on swing range. Cannot exceed the initional local Y.
            transform.localPosition = new Vector3(0f, Mathf.Clamp(transform.localPosition.y, -Mathf.Infinity, initional_localY), 0f);
            
            theta += 0.003f;
            this.transform.Translate(this.transform.up * Mathf.Sin(swing_cycle * 3f * theta) * 0.008f);
        } 

        // If player is crouch walking, then begin sway.
        if(player_movement.isCrouchWalking() && player_movement.IsGrounded() && !player_movement.isSliding) {
            theta += 0.0005f;
            this.transform.Translate(this.transform.up * Mathf.Sin(swing_cycle * 0.0005f * theta) * 0.05f);
        }

        // If player is sliding, set camera offset.
        if(player_movement.isSliding && player_movement.isDuringSliding()) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, sliding_position, Time.deltaTime * 4f);
            if(!go_back) {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, middle_rotation, Time.deltaTime * 8f);
            } else {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, final_rotation, Time.deltaTime * 10f);
            }
            if(transform.localRotation.eulerAngles.z <= 345.5f) {
                go_back = true;
            }
        }
    }
}
