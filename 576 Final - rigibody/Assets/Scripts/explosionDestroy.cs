using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionDestroy : MonoBehaviour
{
    // explosion last
    private float lifeTime = 3.0f;
    // timer 
    private float timer = 0;

    // Update is called once per frame
    void Update()
    {
        // update timer
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
