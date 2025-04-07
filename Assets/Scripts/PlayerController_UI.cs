using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_UI : MonoBehaviour
{
    public Slider staminaSlider;
    float maxStamina = 100f;
    public float currentStamina;
    public TextMeshProUGUI keyText;
    // Start is called before the first frame update
    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
    }

    // Update is called once per frame
    void Update()
    {
        staminaRegen();
    }

    void staminaRegen()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * 0.1f;
            staminaSlider.value = currentStamina;
        }
    }
}
