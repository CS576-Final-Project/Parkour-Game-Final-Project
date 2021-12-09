using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public Gradient gradient;  // set health bar color according to player's current health

    // set the health of the player health bar
    public void SetHealth(float health)
    {
        slider.value = health;
        Debug.Log("health" + health.ToString());
        Debug.Log("slider value" + slider.value);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
