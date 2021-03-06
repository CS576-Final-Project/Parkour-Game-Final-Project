using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustFOVWhenSliding : MonoBehaviour
{
    private Camera player_camera;
    private PlayerMovement player_movement;
    private float initional_FOV, final_FOV;
    // Start is called before the first frame update
    void Start()
    {
        player_camera = (Camera) GameObject.Find("Main Camera").GetComponent<Camera>();
        player_movement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        initional_FOV = 60f;
        final_FOV = 80f;
    }

    // Update is called once per frame
    void Update()
    {
        if(player_movement.isSliding || player_movement.isDuringSliding()) {
            player_camera.fieldOfView = Mathf.Lerp(player_camera.fieldOfView, final_FOV, Time.deltaTime * 1f);
        } else {
            player_camera.fieldOfView = Mathf.Lerp(player_camera.fieldOfView, initional_FOV, Time.deltaTime * 3f);
        }
    }
}
