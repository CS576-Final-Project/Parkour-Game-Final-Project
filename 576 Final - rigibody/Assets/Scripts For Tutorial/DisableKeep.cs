using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableKeep : MonoBehaviour
{
    public GameObject keepRunning;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            keepRunning.SetActive(false);
        }
    }
}
