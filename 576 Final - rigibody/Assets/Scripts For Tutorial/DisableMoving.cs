using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMoving : MonoBehaviour
{
    public GameObject moveInstruction;
    public GameObject crouchInstruction;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            moveInstruction.SetActive(false);
            crouchInstruction.SetActive(true);
        }
    }
}
