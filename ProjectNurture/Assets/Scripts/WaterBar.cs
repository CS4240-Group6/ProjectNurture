using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{
    public Slider slider;

    // change the max amt of water required and reset the slider to 0
    public void SetMaxWaterLevel(int maxWater) {
        slider.maxValue = maxWater;
        slider.value = 0;
    }

    public void SetWaterLevel(int waterLevel) {
        slider.value = waterLevel;
    } 
}
