using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterablePlot : MonoBehaviour
{
    public bool isCanvasVisible = true;

    public int waterLevelGoal = 7;

    public int maxWaterLevel = 10;
    public int currentWaterLevel;

    // index 0 being the most dry material
    public Material[] soilMaterials;

    public WaterBar waterBar = null;
    public GameObject waterBarCanvas = null;

    public GameObject[] plantStages = null;
    private int currentStage = 0;

    void Start()
    {
        currentWaterLevel = 0;
        waterBar.SetMaxWaterLevel(maxWaterLevel);
        ChangeSoilMaterial(0);
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

    public void AddWater(int amount)
    {
        if (currentWaterLevel != maxWaterLevel)
        {
            currentWaterLevel += amount;
            waterBar.SetWaterLevel(currentWaterLevel);
            ChangeSoilMaterial(1);
        }
    }

    // reduce water when time passes
    private void ReduceWater(int amount)
    {
        ChangeSoilMaterial(0);

        if (currentWaterLevel > 0)
        {
            currentWaterLevel -= amount;
            waterBar.SetWaterLevel(currentWaterLevel);
            ChangeSoilMaterial(0);
        }
    }

    private void ChangeSoilMaterial(int index)
    {
        gameObject.GetComponent<Renderer>().material = soilMaterials[index];
    }
}
