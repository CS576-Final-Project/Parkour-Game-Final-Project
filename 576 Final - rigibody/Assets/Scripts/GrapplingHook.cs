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

    public bool fired = false;
    public bool hooked = false;
    public bool play = false;

    private Vector3 hookPoint;

    public float maxDistance;
    private float currentDistance;
    private float HUDMaxDistance;
    
    Rigidbody rb;

    private Vector3 hookInitionalScale;

    public Camera fpsCam;
    private GameObject hookTriggerObj;
    private GameObject HUDTriggerObj;
    public GameObject triggerPoint;
    private GameObject HUDTriggerPoint;
    public Transform orentation;
    public LayerMask hookable;
    public LayerMask hookTrigger;    

    public LayerMask HUDTrigger;

    public RaycastHit hookHit;
    public RaycastHit HUDHit;

    void Awake() {
        lr = GameObject.Find("M1911").GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hookTravelSpeed = 30f;
        playerTravelSpeed = 45f;
        maxDistance = 60f;
        HUDMaxDistance = maxDistance - 10f;

        rb = GetComponent<Rigidbody>(); 

        playerMovement = GetComponent<PlayerMove>();
        animatorControl = GameObject.Find("M1911").GetComponent<M1911AnimatorControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out HUDHit, HUDMaxDistance, HUDTrigger)) {
            HUDTriggerObj = HUDHit.collider.gameObject;
            HUDTriggerPoint = HUDTriggerObj.transform.GetChild(0).gameObject;
            if (HUDTriggerPoint != null) {
                HUDTriggerPoint.SetActive(true);
            }
        } else {
            if (HUDTriggerPoint != null) {
                HUDTriggerPoint.SetActive(false);
            }
        }

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hookHit, maxDistance, hookTrigger)) {
            hookTriggerObj = hookHit.collider.gameObject;
            triggerPoint = hookTriggerObj.transform.GetChild(0).gameObject;
            
            hookPoint = new Vector3(triggerPoint.transform.position.x, triggerPoint.transform.position.y - 8f, triggerPoint.transform.position.z);

            if (Input.GetKeyDown(KeyCode.E) && !fired) {
                playerMovement.hookCurrentDirection = hookHit.point - this.transform.position;
                fired = true;
                play = true;
                animatorControl.animation_controller.Play("M1911 Hook Ready");
                if (HUDTriggerPoint != null) {
                    HUDTriggerPoint.SetActive(false);
                }
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

        if (currentDistance < 10f) {
            StopHooking();
        }
    }

    private void StopHooking() {
        lr.positionCount = 0;
        playerMovement.isRopeCut = true;
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
