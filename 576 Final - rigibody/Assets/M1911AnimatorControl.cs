using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1911AnimatorControl : MonoBehaviour
{
    public Animator animation_controller;
    private GrapplingHook playerHook;

    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        playerHook = GameObject.FindWithTag("Player").GetComponent<GrapplingHook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHook.play) {
            animation_controller.SetTrigger("Hook");
        }
    }
}
