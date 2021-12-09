using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateHookHUD : MonoBehaviour
{
    private Collider selfCollider;
    public RaycastHit[] HUD;
    public RaycastHit selfHit;
    public bool blocked = false;
    public GameObject hitObj;

    // Start is called before the first frame update
    void Start()
    {
        selfCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindWithTag("Player") != null) {
            HUD = GameObject.FindWithTag("Player").GetComponent<GrapplingHUD>().HUDHits;
            hitObj = GameObject.FindWithTag("Player").GetComponent<GrapplingHUD>().hitObj;
        }

        if (beingWatched()) {
            this.transform.GetChild(0).gameObject.SetActive(true);
        } else {
             this.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (hitObj != null) {
            if (hitObj.layer != 9 && hitObj.layer != 10 && hitObj.layer != 12 && hitObj.layer != 13 && hitObj.layer != 14) {
                blocked = true;
            } else {
                blocked = false;
            }
        } else {
            blocked = false;
        }
    }

    private bool beingWatched() {
        if (HUD != null) {
            for (int i = 0; i < HUD.Length ; i++) {
                if (selfCollider == HUD[i].collider && !blocked) {
                    selfHit = HUD[i];
                    return true;
                }
            }
        }
        return false;
    }
}
