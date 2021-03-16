using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterableCropScript : MonoBehaviour
{
    public bool isCanvasVisible = false;

    public int maxWaterLevel = 10;
    public int currentWaterLevel;

    public WaterBarScript waterBar;
    public GameObject waterBarCanvas;

    void Start()
    {
        currentWaterLevel = 0;
        waterBar.SetMaxWaterLevel(maxWaterLevel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddWater(1);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ReduceWater(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetIsWaterBarVisible(!isCanvasVisible);
        }
    }

    void SetIsWaterBarVisible(bool val)
    {
        isCanvasVisible = val;
        waterBarCanvas.SetActive(val);
    }

    void AddWater(int amount)
    {
        if (currentWaterLevel != maxWaterLevel)
        {
            currentWaterLevel += amount;
            waterBar.SetWaterLevel(currentWaterLevel);
        }
    }

    // reduce water when time passes
    void ReduceWater(int amount)
    {
        if (currentWaterLevel > 0)
        {
            currentWaterLevel -= amount;
            waterBar.SetWaterLevel(currentWaterLevel);
        }
    }
}
