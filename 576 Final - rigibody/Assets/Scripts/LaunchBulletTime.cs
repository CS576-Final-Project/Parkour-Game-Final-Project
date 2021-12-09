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

    private bool inPlayed = false;
    private bool outPlayed = false;

    private float cdTimer = 0f;
    private float cdTime = 5f;
    private bool canBullet = true;
    private bool startCount = false;

    void Start() {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
        wallRun = GameObject.FindWithTag("Player").GetComponent<WallRun>();
    }

    void Update()
    {
        if ((!playerMovement.isGrounded() || playerMovement.isSliding) && !wallRun.isWallLeft && !wallRun.isWallRight) {
            if (Input.GetMouseButtonDown(1) && canBullet && Time.timeScale != 0)
            {
                playerMovement.isBulleting = true;
                t = 0;
                if (!inPlayed) {
                    ass.PlayOneShot(clipIn);
                    inPlayed = true;
                }
                canBullet = false;
            }
            if (Input.GetMouseButton(1) && playerMovement.isBulleting)
            {
                t += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.2f, t);
                radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 10, t);
                cae.saturation = Mathf.Lerp(cae.saturation, 0.65f, t);
            }
            if (Input.GetMouseButtonUp(1) || !playerMovement.isBulleting)
            {
                playerMovement.isBulleting = false;
                playerMovement.bulletTimer = 0f;
                t = 1f;
                if (!outPlayed && Time.timeScale < 1 && Time.timeScale != 0f) {
                    ass.PlayOneShot(clipOut);
                    outPlayed = true;
                }
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
                radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 1, t);
                cae.saturation = Mathf.Lerp(cae.saturation, 1f, t);
                startCount = true;
            }
        } else {
            playerMovement.isBulleting = false;
            playerMovement.bulletTimer = 0f;
            t = 1f;
            if (!outPlayed && Time.timeScale < 1 && Time.timeScale != 0f) {
                ass.PlayOneShot(clipOut);
                outPlayed = true;
            }
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, t);
            radiaBlue.Level = Mathf.Lerp(radiaBlue.Level, 1, t);
            cae.saturation = Mathf.Lerp(cae.saturation, 1f, t);

            inPlayed = false;
            outPlayed = false;
        }

        StartCountDown(startCount);
    }

    public void StartBullet() {

    }

    public void StopBullet() {

    }

    private void StartCountDown(bool start) {
        if (start) {
            cdTimer += Time.deltaTime;
            if (cdTimer > cdTime) {
                canBullet = true;
                start = false;
                cdTimer = 0;
            }
        }
    }
}
