using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 move_direction;
    public float velocity, walking_velocity, crouching_velocity, running_velocity, sliding_velocity;
    public CharacterController player_controller;
    public Vector3 fall;
    public float gravity;
    public float jump_height;

    public Transform ground_check;
    public float ground_distance;
    public LayerMask ground_mask;

    public bool isSliding = false;
    private bool slow_down = false;
    private bool capture_direction = false;
    private Vector3 current_direciton = Vector3.zero;
    private WalkingSway sway_data;

    // Start is called before the first frame update
    void Start() {
        velocity = 0f;
        gravity = -20f;
        jump_height = 4f;
        walking_velocity = 10f;
        crouching_velocity = 5f;
        running_velocity = 20f;
        sliding_velocity = 50f;
        ground_distance = 0.3f;
        player_controller = GetComponent<CharacterController>();
        fall = Vector3.zero;
        sway_data = GameObject.FindWithTag("Head").GetComponent<WalkingSway>();
    }

    // Update is called once per frame
    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move_direction = transform.right * h + transform.forward * v;
        player_controller.center = new Vector3(0f, 0f ,0f);
        player_controller.height = 8f;

        fall.y += gravity * Time.deltaTime;
        if(!isSliding) {
            player_controller.Move(new Vector3(move_direction.x * velocity, fall.y, move_direction.z * velocity) * Time.deltaTime);
        } else {
            player_controller.Move(new Vector3(current_direciton.x * velocity, fall.y, current_direciton.z * velocity) * Time.deltaTime);
        }
        

        if(IsGrounded() && fall.y < 0) {
            fall.y = -2f;
        }

        if(IsGrounded()) {
            if(isCrouchStationary() || isCrouchWalking() && !isSliding) {
                player_controller.center = new Vector3(0f, -1f ,0f);
                player_controller.height *= 0.55f;
                Debug.Log("1");
            }

            // Walking movement part.
            if(isWalking() && !isSliding) {
                velocity = walking_velocity;
            }

            // Crouching movement part.
            if(isCrouchWalking() && !isSliding) {
                velocity = crouching_velocity;
            }

            // Running movement part.
            if(isRunning() && !isSliding) {
                velocity = running_velocity;
            }

            // Jumping part.
            if(Input.GetKeyDown(KeyCode.Space) && !isSliding) {
                fall.y = Mathf.Sqrt(jump_height * -2f * gravity);
            }

            // Sliding part.
            if((Input.GetKeyDown(KeyCode.C) && isRunning()) || isSliding) {
                // Get initional sliding direction.
                if(capture_direction) {
                    current_direciton = move_direction;
                }
                capture_direction = false;

                // Instant acceleration when sliding.
                isSliding = true;
                if(velocity < sliding_velocity && !slow_down) {
                    velocity += 110f * Time.deltaTime;
                }
                if(slow_down) {
                    velocity -= 27f * Time.deltaTime; 
                }
                if(velocity <= running_velocity) {
                    isSliding = false;
                    slow_down = false;
                }
                if(velocity >= sliding_velocity) {
                    slow_down = true;
                }
            }

            if(Input.GetKeyDown(KeyCode.C) && isRunning() && !isDuringSliding()) {
                capture_direction = true;
            }
        }
    }

    public bool IsGrounded() {  
        return Physics.CheckSphere(ground_check.position, ground_distance, ground_mask);
    }  

    // Stand part.
    public bool isWalking() {
        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.S)) {
            return true;
        }
        return false;
    }

    public bool isRunning() {
        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && Input.GetKey(KeyCode.LeftShift)) {
            return true;
        }
        return false;
    }

    public bool isStationary() {
        if(!isWalking() && !isRunning()) {
            return true;
        }
        return false;
    }

    // Crouch part.
    public bool isCrouchStationary() {
        if(Input.GetKey(KeyCode.C) && isStationary()) {
            return true;
        }
        return false;
    }

    public bool isCrouchWalking() {
        if(Input.GetKey(KeyCode.C) && isWalking()) {
            return true;
        }
        return false;
    }

    public bool isDuringSliding() {
        if(velocity > (running_velocity + 7.5f) && velocity <= sliding_velocity) {
            return true;
        }
        return false;
    }
}
