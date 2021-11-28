using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1911AnimatorControl : MonoBehaviour
{
    public Animator animationController;
    private GrapplingHook playerHook;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animator>();
        playerHook = GameObject.FindWithTag("Player").GetComponent<GrapplingHook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
