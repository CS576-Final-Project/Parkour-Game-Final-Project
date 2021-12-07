using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableHook : MonoBehaviour
{
    public GameObject hook;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            hook.SetActive(false);
        }
    }
}
