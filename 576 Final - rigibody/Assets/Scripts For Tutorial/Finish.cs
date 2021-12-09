using System.Collections;
using System.Collections.Generic;
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
}
