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

    public bool wallHookFired = false;
    public bool hooked = false;
    public bool play = false;
    private bool captureObj = false;

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
        playerTravelSpeed = 30f;
        wallMaxDistance = 70f;
        wallHUDMaxDistance = wallMaxDistance + 10f;

        rb = GetComponent<Rigidbody>(); 

        playerMovement = GetComponent<PlayerMove>();
        animatorControl = GameObject.Find("M1911").GetComponent<M1911AnimatorControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out wallHookHit, wallMaxDistance)) {
            //wallHookTriggerObj = wallHookHit.collider.gameObject;
            if (!captureObj) {
                wallHookTriggerObj = wallHookHit.collider.gameObject;
                captureObj = true;
            }

            GameObject currObj = wallHookHit.collider.gameObject;
            if (currObj != wallHookTriggerObj && currObj != null) {
                captureObj = false;
            }

            if (wallHookTriggerObj != null) {
                if (wallHookTriggerObj.layer == 12) {
                    triggerPoint = wallHookTriggerObj.transform.GetChild(0).gameObject;
                    wallHookTriggerObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<HUDFade>().canSee = true;
                    
                    hookPoint = triggerPoint.transform.position;

                    if (Input.GetKeyDown(KeyCode.E) && !wallHookFired) {
                        wallHookFired = true;
                        play = true;
                        animatorControl.animationController.Play("M1911 Hook Ready");
                    }
                }
            }

        } else {
            if (wallHookTriggerObj != null && wallHookTriggerObj.layer == 12)
                wallHookTriggerObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<HUDFade>().canSee = false;
        }

        if (animatorControl.animationController.GetCurrentAnimatorStateInfo(0).IsName("M1911 Hook Ready") && animatorControl.animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && wallHookFired) {
            animatorControl.animationController.Play("M1911");
            hooked = true; 
        }   
        if (hooked) {
            StartHooking();
            playerMovement.speedLine.Play();
            gunTip.parent.transform.rotation = Quaternion.LookRotation(triggerPoint.transform.position - gunTip.position) * Quaternion.LookRotation(gunTip.parent.GetComponent<SwitchWeaponPlace>().initialForward, Vector3.up);
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    private void StartHooking() {
        rb.useGravity = false;
        this.transform.position = Vector3.MoveTowards(this.transform.position, hookPoint, playerTravelSpeed * Time.deltaTime * 5f);
        playerMovement.wallHookCurrentDirection = hookPoint - this.transform.position;
        currentDistance = Vector3.Distance(this.transform.position, hookPoint);

        lr.positionCount = 2;

        if (currentDistance < 7f) {
            StopHooking();
        }
    }

    private void StopHooking() {
        lr.positionCount = 0;
        playerMovement.isWallRopeCut = true;
        rb.useGravity = true;
        hooked = false;
        wallHookFired = false;
        play = false;
        captureObj = false;
    }

    private void DrawRope() {
        if (!hooked) return;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, triggerPoint.transform.position);
    }
}
