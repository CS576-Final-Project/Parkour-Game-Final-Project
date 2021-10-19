using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustFOVWhenSliding : MonoBehaviour
{
    private Camera player_camera;
    private PlayerMove player_movement;
    private float initional_FOV, final_FOV;
    // Start is called before the first frame update
    void Start()
    {
        player_camera = (Camera) GameObject.Find("Main Camera").GetComponent<Camera>();
        player_movement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        initional_FOV = 60f;
        final_FOV = 85f;
    }

    // Update is called once per frame
    void Update()
    {
        if(player_movement.isSliding) {
            player_camera.fieldOfView = Mathf.Lerp(player_camera.fieldOfView, final_FOV, Time.deltaTime * 1f);
        } else {
            player_camera.fieldOfView = Mathf.Lerp(player_camera.fieldOfView, initional_FOV, Time.deltaTime * 3f);
        }
    }
}
