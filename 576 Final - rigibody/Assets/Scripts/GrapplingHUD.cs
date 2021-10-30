using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHUD : MonoBehaviour
{
    public Camera fpsCam;
    public LayerMask HUDTrigger;
    public RaycastHit[] HUDHits;
    public float HUDMaxDistance;

    // Start is called before the first frame update
    void Start()
    {
        HUDMaxDistance = 80f;
        HUDHits = new RaycastHit[]{};
    }

    // Update is called once per frame
    void Update()
    {
        HUDHits = Physics.RaycastAll(fpsCam.transform.position, fpsCam.transform.forward, HUDMaxDistance, HUDTrigger);
    }
}
