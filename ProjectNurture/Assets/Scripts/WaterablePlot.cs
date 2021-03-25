using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterablePlot : MonoBehaviour
{
    public int waterLevelCurrent = 0;
    public int waterLevelMax = 10;
    public int waterLevelGoal = 7;
    public float waterEvaporationPeriod = 15f; // water level is reduced by 1 every 15s

    public GameObject waterBarCanvas = null;
    public WaterBar waterBar = null;
    public bool isCanvasVisible = true;

    public AudioSource successSoundEffect = null;

    // index 0 contains the dry soil material, index 1 contains the wet soil material
    public Material[] soilMaterials;

    public GameObject[] plantStages = null;
    private int currentStage = 0;

    void Start()
    {
        waterBarCanvas.SetActive(isCanvasVisible);
        
        waterBar.SetMaxWaterLevel(waterLevelMax, waterLevelGoal);
        waterBar.SetWaterLevel(waterLevelCurrent);

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
            if (waterLevelCurrent > 0)
            {
                ReduceWater(1);
            }

            yield return new WaitForSeconds(waterEvaporationPeriod);
        }
    }

    public void AddWater(int amount)
    {
        if (waterLevelCurrent != waterLevelMax)
        {
            waterLevelCurrent += amount;
            waterBar.SetWaterLevel(waterLevelCurrent);

            if (waterLevelCurrent == waterLevelGoal)
            {
                successSoundEffect.Play();
            }

            UpdateSoilMaterial();
        }

    }

    private void ReduceWater(int amount)
    {
        if (waterLevelCurrent > 0)
        {
            waterLevelCurrent -= amount;
            waterBar.SetWaterLevel(waterLevelCurrent);
            UpdateSoilMaterial();
        }
    }

    private void UpdateSoilMaterial()
    {
        float ratio = waterLevelCurrent / (float) waterLevelMax;
        Color newColor = Color.Lerp(soilMaterials[0].color, soilMaterials[1].color, ratio);

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColor);
    }

}
