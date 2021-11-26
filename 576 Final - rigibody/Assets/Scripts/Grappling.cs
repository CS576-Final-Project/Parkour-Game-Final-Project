using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip;
    public Transform fpsCam;
    public Transform player;
    public Transform grappleCheck;
    private float maxDistance = 1000f;
    private SpringJoint joint;
    private Camera cam;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = (Camera)GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            StartGrapple();
        } else if (Input.GetMouseButtonUp(1)) {
            StopGrapple();
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    private void StartGrapple() {
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.95f;
            joint.minDistance = distanceFromPoint * 0.05f;

            joint.spring = 1f;
            joint.damper = 40f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    private void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }
    
    private void DrawRope() {
        if (!joint) return;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }
}
