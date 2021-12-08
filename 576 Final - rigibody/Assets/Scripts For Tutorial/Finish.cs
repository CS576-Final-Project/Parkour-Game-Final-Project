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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            sight.SetActive(false);
            deathCam.GetComponent<AudioListener>().enabled = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PandO.SetActive(false);
            finishUI.SetActive(true);
        }
    }

    public void Restart() {
        SceneManager.LoadScene(0);
    }

    public void BackMenu() {
    }
}
