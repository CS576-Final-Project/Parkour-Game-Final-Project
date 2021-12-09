using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;  // text to show time passed
    private float timeUsed;  // passed time
    
    // Start is called before the first frame update
    void Start()
    {
        timeUsed = 0;  // initialize time passed
    }

    // Update is called once per frame
    void Update()
    {
        timeUsed += Time.deltaTime;  // update time passed
        timeText.text = "Time Passed: " + Math.Round(timeUsed, 0) + " sec";  // update time text shown
    }
}
