using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathT : MonoBehaviour
{
    public GameObject PandO;
    public GameObject player;
    public GameObject body;
    private PlayerHealth playerHealth;

    public GameObject deathUI;
    public GameObject hookInstruction;

    public GameObject deathCam;
    public GameObject sight;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = body.GetComponent<PlayerHealth>();
        // PandO = Instantiate(PandO_Prefeb);
        // PandO.transform.position = new Vector3(3.5f, 14.2f, 2.75f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.playerDie) {
            sight.SetActive(false);
            deathCam.GetComponent<AudioListener>().enabled = true;
            PandO.SetActive(false);
            deathUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.R)) {
                PandO.SetActive(true);
                player.transform.position = new Vector3(58.17828f, -41f, -84f);
                player.transform.eulerAngles = transform.eulerAngles + 180f * Vector3.up;
                playerHealth.health = 100;
                playerHealth.playerDie = false;
                hookInstruction.SetActive(true);
                deathCam.GetComponent<AudioListener>().enabled = false;
                sight.SetActive(true);
            }
        } else {
            deathUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14 && playerHealth.health > 0) {
            GameObject player = other.gameObject;
            playerHealth = body.GetComponent<PlayerHealth>();
            playerHealth.health -= 100;
        }
    }
}
