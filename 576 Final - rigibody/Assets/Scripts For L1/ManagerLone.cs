using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLone : MonoBehaviour
{
    public GameObject body;
    private PlayerHealth playerHealth;

    public GameObject deathUI;
    public GameObject PandO;
    public GameObject player;
    public GameObject deathCam;
    public GameObject sight;
    public GameObject finish;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = body.GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.playerDie) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            sight.SetActive(false);
            deathCam.GetComponent<AudioListener>().enabled = true;
            PandO.SetActive(false);
            deathUI.SetActive(true);
            finish.SetActive(false);
            
            if (Input.GetKeyDown(KeyCode.R)) {
                // Cursor.visible = false;
                // Cursor.lockState = CursorLockMode.Locked;
                // PandO.SetActive(true);
                // player.transform.position = new Vector3(484.5533f, 173.25f, 149.7674f);
                // playerHealth.health = 100;
                // playerHealth.playerDie = false;
                // deathCam.GetComponent<AudioListener>().enabled = false;
                // sight.SetActive(true);
                // finish.SetActive(true);
                SceneManager.LoadScene(3);
            }
        } else {
            deathUI.SetActive(false);
        }
    }
}
