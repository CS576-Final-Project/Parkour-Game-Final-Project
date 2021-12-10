using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDeath : MonoBehaviour
{
    public GameObject PandO;
    public GameObject player;
    public GameObject body;
    private PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = body.GetComponent<PlayerHealth>();;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 14) {
            playerHealth.health = 0;
        }
    }
}
