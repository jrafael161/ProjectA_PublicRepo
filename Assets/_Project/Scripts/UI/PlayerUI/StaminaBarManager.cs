using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarManager : MonoBehaviour
{
    public Slider slider;

    public void SetMaxStamina(float maxStamina)
    {
        slider.maxValue = maxStamina;
        slider.value = maxStamina;
    }

    public void SetCurrentStamina(float currentStamina)
    {
        slider.value = currentStamina;
    }
}
