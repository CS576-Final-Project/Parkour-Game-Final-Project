using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // public Transform ground_check;
    // public float ground_distance = 0.3f;
    // public LayerMask ground_mask;
    private float player_height = 2f;
    private MoveSway sway;

    [Header("Movement")]
    public float walking_velocity;
    public float crouching_velocity;
    public float running_velocity;
    public float sliding_velocity;
    public float movement_multiplier;
    [SerializeField] float air_multiplier;

    [Header("Jump")]
    public float jump_force;

    [Header("Keybinds")]
    [SerializeField] KeyCode jump_key = KeyCode.Space;
    [SerializeField] KeyCode walk_forward_key = KeyCode.W;
    [SerializeField] KeyCode walk_left_key = KeyCode.A;
    [SerializeField] KeyCode walk_backward_key = KeyCode.S;
    [SerializeField] KeyCode walk_right_key = KeyCode.D;
    [SerializeField] KeyCode crouch_key = KeyCode.C;
    [SerializeField] KeyCode run_key = KeyCode.LeftShift;


    float rb_ground_drag = 6f;
    float rb_air_drag = 2f;
    float rb_slid_drag = 12f;

    float horizontal_movement;
    float vertical_movement;

    Vector3 move_direction;

    Rigidbody rb;

    private bool capture_direction = false;
    public bool isSliding = false;
    private Vector3 current_direction = Vector3.zero;
    public float time = 0f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //ground_distance = 0.4f;

        walking_velocity = 7f;
        crouching_velocity = 3f;
        running_velocity = 15f;
        sliding_velocity = 40f;
        movement_multiplier = 8f;
        air_multiplier = 0.4f;

        jump_force = 10f;

        sway = GameObject.Find("Head").GetComponent<MoveSway>();
    }

    void Update() {
        Inputs();
        ControlDrag();
        
        if(IsGrounded()) {
            if(Input.GetKeyDown(jump_key) && !isCrouchWalking() && !isCrouchStationary()) {
                Jump();
            }
        }
        
        if(Input.GetKeyDown(crouch_key) && isRunning()) {
            capture_direction = true;
        }

        if((Input.GetKeyDown(crouch_key) && isRunning()) || isSliding) {
            Slid();
            time += Time.deltaTime;
            if(time >= 0.75f) {
                isSliding = false;
            }
        } else if(isRunning()){
            time = 0f;
        }
    }

    private void Inputs() {
        horizontal_movement = Input.GetAxisRaw("Horizontal");
        vertical_movement = Input.GetAxisRaw("Vertical");

        move_direction = transform.forward * vertical_movement + transform.right * horizontal_movement;
    }

    private void ControlDrag() {
        if(IsGrounded() && !isSliding) {
            rb.drag = rb_ground_drag;
        } else if(!IsGrounded()){
            rb.drag = rb_air_drag;
        } else if(IsGrounded() && isSliding) {
            rb.drag = rb_slid_drag;
        }
    }

    private void FixedUpdate() {
        if(isWalking()) {
            PlayerWalking();
        }

        if(isRunning()) {
            PlayerRunning();
        }

        if(IsGrounded()) {
            if(isCrouchWalking()) {
                PlayerCrouchWalking();
            }
        }
    }

    private void PlayerWalking() {
        if(IsGrounded()) {
            rb.AddForce(move_direction.normalized * walking_velocity * movement_multiplier, ForceMode.Acceleration);
        } else if(!IsGrounded()){
            rb.AddForce(move_direction.normalized * walking_velocity * movement_multiplier * air_multiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerRunning() {
        if(IsGrounded()) {
            rb.AddForce(move_direction.normalized * running_velocity * movement_multiplier, ForceMode.Acceleration);
        } else if(!IsGrounded()){
            rb.AddForce(move_direction.normalized * running_velocity * movement_multiplier * air_multiplier, ForceMode.Acceleration);
        }
    }

    private void PlayerCrouchWalking() {
        if(IsGrounded()) {
            rb.AddForce(move_direction.normalized * crouching_velocity * movement_multiplier, ForceMode.Acceleration);
        }
    }

    public bool IsGrounded() {  
        return Physics.Raycast(transform.position, Vector3.down, player_height / 2 + 0.1f);
    } 

    private void Jump() {
        rb.AddForce(transform.up * jump_force, ForceMode.Impulse);
    }

    private void Slid() {
        if(capture_direction) {
            current_direction = move_direction;
        }
        if(sway.can_slide) {
            isSliding = true;
        }
        capture_direction = false;
        rb.AddForce(current_direction.normalized * 1.5f, ForceMode.VelocityChange);
    }

    // Stand part.
    public bool isWalking() {
        if((Input.GetKey(walk_forward_key) || Input.GetKey(walk_left_key) || Input.GetKey(walk_right_key)) && !Input.GetKey(run_key) && !Input.GetKey(crouch_key) || Input.GetKey(walk_backward_key)) {
            return true;
        }
        return false;
    }

    public bool isRunning() {
        if((Input.GetKey(walk_forward_key) || Input.GetKey(walk_left_key) || Input.GetKey(walk_right_key)) && Input.GetKey(run_key) && !isSliding) {
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
        if(Input.GetKey(crouch_key) && isStationary()) {
            return true;
        }
        return false;
    }

    public bool isCrouchWalking() {
        if(Input.GetKey(crouch_key) && (Input.GetKey(walk_forward_key) || Input.GetKey(walk_left_key) || Input.GetKey(walk_right_key) || Input.GetKey(walk_backward_key))) {
            return true;
        }
        return false;
    }
}
