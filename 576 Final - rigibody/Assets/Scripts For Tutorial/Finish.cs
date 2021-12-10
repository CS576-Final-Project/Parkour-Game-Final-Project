using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    public GameObject finishUI;
    public GameObject PandO;

    public GameObject deathCam;
    public GameObject sight;

    public GameObject pause;
    public bool paused = false;

    public GameObject instruction;
    
    public TextMeshProUGUI timeTextSight;  // time text on sight canvas
    public TextMeshProUGUI timeTextFinish;  // time text on menu canvas
    public TextMeshProUGUI rank;  // rank on menu canvas

    private bool finished = false;
    private bool instructionOpen = false;
    private bool captureTimeScale = false;
    private float currTimeScale;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !paused && !finished) {
            paused = true;
            captureTimeScale = true;
        }   else if (!paused && !finished) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pause.SetActive(false);
            sight.SetActive(true);
            paused = false;
        }

        if (paused) {
            if (captureTimeScale) {
                currTimeScale = Time.timeScale;
                captureTimeScale = false;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            sight.SetActive(false);
            
            Time.timeScale = 0;
            if (!instructionOpen)
                pause.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            finished = true;
            sight.SetActive(false);
            deathCam.GetComponent<AudioListener>().enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PandO.SetActive(false);
            
            string[] strArr = new string[4];
            string timeStr = timeTextSight.text.Split(" "[0])[2];
            // update time text on finish UI
            timeTextFinish.text = "Time Used: " + timeStr + " sec";  // update time text shown
            int timeInt = int.Parse(timeStr);
            // calculate rank based on time
            if (timeInt <= 60)
            {
                rank.text = "S";
            }
            else if (timeInt <= 80)
            {
                rank.text = "A";
            }
            else if (timeInt <= 100)
            {
                rank.text = "B";
            }
            else if (timeInt <= 120)
            {
                rank.text = "C";
            }
            else 
            {
                rank.text = "D";
            }
            finishUI.SetActive(true);
        }
    }

    public void Restart() {
        finished = false;
        SceneManager.LoadScene(1);
    }

    public void BackMenu() {
        SceneManager.LoadScene(2);
    }

    public void Resume() {
        Time.timeScale = currTimeScale;
        paused = false;
    }

    public void OpenInstruction() {
        instructionOpen = true;
        instruction.SetActive(true);
        pause.SetActive(false);
    }

    public void ReturnPause() {
        instructionOpen = false;
        instruction.SetActive(false);
        pause.SetActive(true);
    }

    public void RestartL1() {
        finished = false;
        SceneManager.LoadScene(3);
    }
}
