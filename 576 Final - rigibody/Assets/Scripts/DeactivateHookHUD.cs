using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateHookHUD : MonoBehaviour
{
    private Collider selfCollider;
    public RaycastHit[] HUD;
    public RaycastHit selfHit;

    // Start is called before the first frame update
    void Start()
    {
        selfCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        HUD = GameObject.FindWithTag("Player").GetComponent<GrapplingHUD>().HUDHits;
        if (beingWatched()) {
            this.transform.GetChild(0).gameObject.SetActive(true);
        } else {
             this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private bool beingWatched() {
        for (int i = 0; i < HUD.Length ; i++) {
            if (selfCollider == HUD[i].collider) {
                selfHit = HUD[i];
                return true;
            }
        }
        return false;
    }
}
