using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableJumpDown : MonoBehaviour
{
    public GameObject jumpDown;
    public GameObject moveInstruction;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            jumpDown.SetActive(false);
            moveInstruction.SetActive(true);
        }
    }
}
