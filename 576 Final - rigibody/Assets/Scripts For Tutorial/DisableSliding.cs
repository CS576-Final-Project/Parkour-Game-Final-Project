using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSliding : MonoBehaviour
{
    public GameObject slideInstruction;
    public GameObject steepInstruction;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            slideInstruction.SetActive(false);
            steepInstruction.SetActive(true);
        }
    }
}
