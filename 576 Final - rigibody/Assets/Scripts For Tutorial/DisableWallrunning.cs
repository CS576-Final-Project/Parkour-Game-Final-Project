using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWallrunning : MonoBehaviour
{
    public GameObject wallRunInstruction;
    public GameObject keepRunning;
    public GameObject slideInstruction;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            wallRunInstruction.SetActive(false);
            slideInstruction.SetActive(true);
            keepRunning.SetActive(true);
        }
    }
}
