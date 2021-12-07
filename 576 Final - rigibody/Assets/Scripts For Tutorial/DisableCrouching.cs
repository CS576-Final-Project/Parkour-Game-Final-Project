using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCrouching : MonoBehaviour
{
    public GameObject wallRunInstruction;
    public GameObject crouchInstruction;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            crouchInstruction.SetActive(false);
            wallRunInstruction.SetActive(true);
        }
    }
}
