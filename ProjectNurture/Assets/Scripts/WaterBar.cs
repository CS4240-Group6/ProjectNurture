using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterBar : MonoBehaviour
{
    public Slider progressSlider;
    public GameObject goalSliderContainer;
    private Slider goalSlider;

    private float componentHeight;

    private void Awake()
    {
        goalSlider = goalSliderContainer.GetComponent<Slider>();
    }

    // change the max amt of water required and reset the slider to 0
    // change the goal level as well
    public void SetMaxWaterLevel(int maxWater, int newGoal) {
        progressSlider.maxValue = maxWater;
        goalSlider.maxValue = maxWater;
        progressSlider.value = 0;

        goalSlider.maxValue = maxWater;
        goalSlider.value = newGoal;
    }

    public void SetWaterLevel(int waterLevel) {
        progressSlider.value = waterLevel;
    }

    public void SetGoalLevel(int goalLevel) {
        goalSlider.value = goalLevel;
    }
}
