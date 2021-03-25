using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterablePlot : MonoBehaviour
{
    public bool isCanvasVisible = true;

    public int waterLevelGoal = 7;
    public int maxWaterLevel = 10;
    
    public int currentWaterLevel = 0;
    public float waterEvaporationSpeed = 5f;

    // index 0 contains the dry soil material, index 1 contains the wet soil material
    public Material[] soilMaterials;

    public WaterBar waterBar = null;
    public GameObject waterBarCanvas = null;

    public GameObject[] plantStages = null;
    private int currentStage = 0;


    void Start()
    {
        waterBarCanvas.SetActive(isCanvasVisible);
        
        waterBar.SetMaxWaterLevel(maxWaterLevel, waterLevelGoal);
        waterBar.SetWaterLevel(currentWaterLevel);

        StartCoroutine(ReduceWaterOverTime());
        UpdateSoilMaterial();
    }

    void Update()
    {
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

    private IEnumerator ReduceWaterOverTime()
    {
        while (gameObject.activeSelf)
        {
            if (currentWaterLevel > 0)
            {
                ReduceWater(1);
            }

            yield return new WaitForSeconds(waterEvaporationSpeed);
        }
    }

    public void AddWater(int amount)
    {
        if (currentWaterLevel != maxWaterLevel)
        {
            currentWaterLevel += amount;
            waterBar.SetWaterLevel(currentWaterLevel);
            UpdateSoilMaterial();
        }

    }

    // reduce water when time passes
    private void ReduceWater(int amount)
    {
        if (currentWaterLevel > 0)
        {
            currentWaterLevel -= amount;
            waterBar.SetWaterLevel(currentWaterLevel);
            UpdateSoilMaterial();
        }
    }

    private void UpdateSoilMaterial()
    {
        float ratio = currentWaterLevel / (float) maxWaterLevel;
        Color newColor = Color.Lerp(soilMaterials[0].color, soilMaterials[1].color, ratio);
        ChangeSoilColor(newColor);
    }

    private void ChangeSoilColor(Color c)
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", c);
    }
}
