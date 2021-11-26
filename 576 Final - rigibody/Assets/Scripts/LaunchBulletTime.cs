using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBulletTime : MonoBehaviour
{
    public float theTimeScale;

    public RadiaBlur radiaBlue;
    public ColorAdjustEffect cae;


    public AudioSource ass;
    public AudioClip clipIn;
    public AudioClip clipOut;

    public float t;

    public PlayerMove playerMovement;
    public WallRun wallRun;

    void Start() {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        wallRun = GameObject.FindWithTag("Player").GetComponent<WallRun>();
    }

    void Update()
    {
        if (!playerMovement.isGrounded() && !wallRun.isWallLeft && !wallRun.isWallRight) {
            if (Input.GetMouseButtonDown(1))
            {
                playerMovement.isBulleting = true;
                t = 0;
                ass.PlayOneShot(clipIn);
            }
            if (Input.GetMouseButton(1) && playerMovement.isBulleting)
            {
                t += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.25f, t);
                radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 15, t);
                cae.saturation = Mathf.Lerp(cae.saturation, 0.5f, t);
            }
            if (Input.GetMouseButtonUp(1) || !playerMovement.isBulleting)
            {
                playerMovement.isBulleting = false;
                playerMovement.bulletTimer = 0f;
                t = 1f;
                ass.PlayOneShot(clipOut);
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
                radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 1, t);
                cae.saturation = Mathf.Lerp(cae.saturation, 1f, t);

            }
        } else {
            playerMovement.isBulleting = false;
            playerMovement.bulletTimer = 0f;
            t = 1f;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
            radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 1, t);
            cae.saturation = Mathf.Lerp(cae.saturation, 1f, t);
        }
    }
}
