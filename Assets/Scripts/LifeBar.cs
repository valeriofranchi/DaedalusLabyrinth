using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Slider healthOnBar;
    public Gradient colourGradient;
    public Image amount;

    public void InitializeHealth(int maxHealth)
    {
        // starts health at max value 
        healthOnBar.maxValue = maxHealth;
        healthOnBar.value = maxHealth;

        colourGradient.Evaluate(1.0f); // 1 -> max 
    }

    public void ModifyHealth(int updatedHealth)
    {
        // modifies the value 
        healthOnBar.value = updatedHealth;
        amount.color = colourGradient.Evaluate(healthOnBar.normalizedValue); // depends on slider value 
    }
}
