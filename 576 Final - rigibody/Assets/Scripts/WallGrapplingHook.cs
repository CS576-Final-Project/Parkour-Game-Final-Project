using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrapplingHook : MonoBehaviour
{
    private PlayerMove playerMovement;
    private M1911AnimatorControl animatorControl;
    private LineRenderer lr;

    public Transform gunTip;

    public float playerTravelSpeed;

    public bool fired = false;
    public bool hooked = false;
    public bool play = false;

    private Vector3 hookPoint;

    public float wallMaxDistance;
    private float currentDistance;
    private float wallHUDMaxDistance;

    Rigidbody rb;

    public Camera fpsCam;
    private GameObject wallHookTriggerObj;
    private GameObject HUDTriggerObj;
    public GameObject triggerPoint;
    private GameObject HUDTriggerPoint;
    public Transform orentation;
    public LayerMask hookable;
    public LayerMask wallHookTrigger;
    public LayerMask HUDTrigger;

    public RaycastHit wallHookHit;
    public RaycastHit HUDHit;

    void Awake() {
        lr = GameObject.Find("M1911").GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTravelSpeed = 35f;
        wallMaxDistance = 80f;
        wallHUDMaxDistance = wallMaxDistance - 10f;

        rb = GetComponent<Rigidbody>(); 

        playerMovement = GetComponent<PlayerMove>();
        animatorControl = GameObject.Find("M1911").GetComponent<M1911AnimatorControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out wallHookHit, wallMaxDistance, wallHookTrigger)) {
            wallHookTriggerObj = wallHookHit.collider.gameObject;
            triggerPoint = wallHookTriggerObj.transform.GetChild(0).gameObject;
            
            hookPoint = triggerPoint.transform.position;

            if (Input.GetKeyDown(KeyCode.E) && !fired) {
                playerMovement.hookCurrentDirection = wallHookHit.point - this.transform.position;
                fired = true;
                play = true;
                animatorControl.animation_controller.Play("M1911 Hook Ready");
            }

            if (animatorControl.animation_controller.GetCurrentAnimatorStateInfo(0).IsName("M1911 Hook Ready") && animatorControl.animation_controller.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
                animatorControl.animation_controller.Play("M1911");
                play = false;
                hooked = true; 
            }   

            if (hooked) {
                StartHooking();
            }
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    private void StartHooking() {
        rb.useGravity = false;
        this.transform.position = Vector3.MoveTowards(this.transform.position, hookPoint, playerTravelSpeed * Time.deltaTime * 5f);
        currentDistance = Vector3.Distance(this.transform.position, hookPoint);

        lr.positionCount = 2;

        if (currentDistance < 5f) {
            StopHooking();
        }
    }

    private void StopHooking() {
        lr.positionCount = 0;
        playerMovement.isWallRopeCut = true;
        rb.useGravity = true;
        hooked = false;
        fired = false;
    }

    private void DrawRope() {
        if (!hooked) return;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, triggerPoint.transform.position);
    }
}
