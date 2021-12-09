using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{   
    private PlayerMove playerMovement;
    private M1911AnimatorControl animatorControl;
    private LineRenderer lr;

    public Transform gunTip;

    public float hookTravelSpeed;
    public float playerTravelSpeed;

    public bool hookFired = false;
    public bool hooked = false;
    public bool play = false;
    private bool captureObj = false;

    private Vector3 hookPoint;

    public float maxDistance;
    public float wallMaxDistance;
    private float currentDistance;
    private float HUDMaxDistance;
    private float wallHUDMaxDistance;
    
    Rigidbody rb;

    public Camera fpsCam;
    private GameObject hookTriggerObj;
    private GameObject HUDTriggerObj;
    private GameObject wallHookTriggerObj;
    public GameObject triggerPoint;
    private GameObject HUDTriggerPoint;
    public Transform orentation;
    public LayerMask hookable;
    public LayerMask hookTrigger;
    public LayerMask wallHookTrigger;    

    public LayerMask HUDTrigger;

    public RaycastHit hookHit;
    public RaycastHit HUDHit;
    public RaycastHit wallHookHit;

    void Awake() {
        lr = GameObject.Find("M1911").GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hookTravelSpeed = 30f;
        playerTravelSpeed = 35f;
        maxDistance = 60f;
        wallMaxDistance = 60f;
        HUDMaxDistance = maxDistance - 10f;

        rb = GetComponent<Rigidbody>(); 

        playerMovement = GetComponent<PlayerMove>();
        animatorControl = GameObject.Find("M1911").GetComponent<M1911AnimatorControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hookHit, maxDistance, hookTrigger)) {
            if (!captureObj) {
                hookTriggerObj = hookHit.collider.gameObject;
                captureObj = true;
            }

            GameObject currObj = hookHit.collider.gameObject;
            if (currObj != hookTriggerObj && currObj != null) {
                captureObj = false;
            }

            triggerPoint = hookTriggerObj.transform.GetChild(0).gameObject;
            hookTriggerObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<HUDFade>().canSee = true;

            
            hookPoint = new Vector3(triggerPoint.transform.position.x, triggerPoint.transform.position.y - 8f, triggerPoint.transform.position.z);

            if (Input.GetKeyDown(KeyCode.E) && !hookFired) {
                playerMovement.hookCurrentDirection = hookHit.point - this.transform.position;
                hookFired = true;
                play = true;
                animatorControl.animationController.Play("M1911 Hook Ready");
            }
        } else {
            if (hookTriggerObj != null)
                hookTriggerObj.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<HUDFade>().canSee = false;
        }
        
        if (animatorControl.animationController.GetCurrentAnimatorStateInfo(0).IsName("M1911 Hook Ready") && animatorControl.animationController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && hookFired) {
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
        currentDistance = Vector3.Distance(this.transform.position, hookPoint);

        lr.positionCount = 2;

        if (currentDistance < 10f) {
            StopHooking();
        }
    }

    private void StopHooking() {
        lr.positionCount = 0;
        playerMovement.isRopeCut = true;
        rb.useGravity = true;
        hooked = false;
        hookFired = false;
        play = false;
        captureObj = false;
    }

    private void DrawRope() {
        if (!hooked) return;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, triggerPoint.transform.position);
    }
}
