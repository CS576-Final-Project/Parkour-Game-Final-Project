using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    private float y;
    public float delay;
    private float timer = 0;
    private bool begin = false;

    // Start is called before the first frame update
    void Start()
    {
        y = transform.position.y;
        delay = Random.Range(0.25f, 2.25f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= delay && !begin) {
            begin = true;
            timer = 0;
        }

        if (begin) {
            y += Mathf.Sin(timer * Mathf.PI * 0.85f) * 0.04f;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
