using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustFOVWhenSliding : MonoBehaviour
{
    private Camera playerCamera;
    private PlayerMove playerMovement;
    private float initionalFOV, finalFOV;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = (Camera) GameObject.Find("Main Camera").GetComponent<Camera>();
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        initionalFOV = 60f;
        finalFOV = 80f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isSliding) {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, finalFOV, Time.deltaTime * 1f);
        } else {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initionalFOV, Time.deltaTime * 3f);
        }
    }
}
