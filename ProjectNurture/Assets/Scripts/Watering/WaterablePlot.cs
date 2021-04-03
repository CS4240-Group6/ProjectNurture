using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterablePlot : MonoBehaviour
{
    public int waterLevelBuffer = 2; // if player waters more than goal + buffer, plant dies
    public float waterEvaporationPeriod = 60f; // water level is reduced by 1 every 60s

    public AudioClip successSoundEffect;
    public AudioClip warningSoundEffect;

    // index 0 contains the dry soil material, index 1 contains the wet soil material
    public Material[] soilMaterials;

    private static string CLAY_LOAM = "CLAY_LOAM";
    private static string WELL_DRAINED = "WELL_DRAINED";

    // soil type can either be WELL_DRAINED or CLAY_LOAM
    public string soilType = WELL_DRAINED;

    private int waterLevelCurrent = 0;
    private int waterLevelMax = 10;
    private int waterLevelGoal = 0;

    private GameObject waterBar;
    private GameObject crossIcon;
    private GameObject witherIcon;
    private GameObject noSeedIcon;
    private GameObject noSoilIcon;
    private GameObject noWellDrainedSoilIcon;
    private GameObject noClayLoamSoilIcon;

    private WaterBar waterBarScript;
    private SoundController soundController;
    private PlantStageController plantStageController;
    private Soil soilMound;

    private bool isPlotWaterable = false;

    private void Awake()
    {
        soundController = GetComponent<SoundController>();
        plantStageController = GetComponent<PlantStageController>();

        GameObject canvas = transform.GetChild(0).gameObject;

        waterBar = canvas.transform.GetChild(0).gameObject;
        crossIcon = canvas.transform.GetChild(1).gameObject;
        witherIcon = canvas.transform.GetChild(2).gameObject;
        noSeedIcon = canvas.transform.GetChild(3).gameObject;
        noSoilIcon = canvas.transform.GetChild(4).gameObject;
        noWellDrainedSoilIcon = canvas.transform.GetChild(5).gameObject;
        noClayLoamSoilIcon = canvas.transform.GetChild(6).gameObject;

        soilMound = gameObject.transform.GetChild(1).GetComponent<Soil>(); // Get script of soil mound

        waterBarScript = waterBar.GetComponent<WaterBar>();
    }

    private void Start()
    {
        waterBarScript.SetMaxWaterLevel(waterLevelMax);
        waterBarScript.SetWaterGoal(waterLevelGoal);
        waterBarScript.SetWaterLevel(waterLevelCurrent);

        SetIsPlotWaterable(isPlotWaterable);

        StartCoroutine(ReduceWaterOverTime());
        UpdateSoilMaterial();
    }

    public void SetIsPlotWaterable(bool val)
    {
        waterBar.SetActive(val);
        isPlotWaterable = val;
    }

    public void SetWaterGoal(int goal)
    {
        waterLevelGoal = goal;
        waterBarScript.SetWaterGoal(goal);
    }

    public void ResetWater()
    {
        waterLevelCurrent = 0;
        waterBarScript.SetWaterLevel(waterLevelCurrent);
        UpdateSoilMaterial();
    }

    public string GetSoilType()
    {
        return soilType;
    }

    public void DisplayWrongSoilUI()
    {
        noClayLoamSoilIcon.SetActive(soilType != CLAY_LOAM);
        noWellDrainedSoilIcon.SetActive(soilType != WELL_DRAINED);
    }

    public void AddWater(int amount)
    {
        if (!isPlotWaterable)
        {
            WarnIncompleteStep();
        }
        else if (waterLevelCurrent != waterLevelMax)
        {
            waterLevelCurrent += amount;
            waterBarScript.SetWaterLevel(waterLevelCurrent);

            if (waterLevelCurrent == waterLevelGoal)
            {
                WaterSuccess();
            }

            if (waterLevelCurrent > waterLevelGoal)
            {
                WaterWarning();
            }

            if (waterLevelCurrent > waterLevelGoal + waterLevelBuffer)
            {
                WaterFail();
            }

            UpdateSoilMaterial();
        }

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

    private void WaterSuccess()
    {
        soundController.PlayAudio(successSoundEffect);
        plantStageController.SetIsWatered(true);
    }

    private void WarnIncompleteStep()
    {
        if (!plantStageController.GetHasSeed())
        {
            noSeedIcon.SetActive(true);
        } 
        else if (!plantStageController.GetIsSeedCovered())
        {
            noSoilIcon.SetActive(true);
        }
        else
        {
            WaterWarning();
        }
    }

    private void WaterWarning()
    {
        soundController.PlayAudio(warningSoundEffect);
        crossIcon.SetActive(true);
    }
    private void WaterFail()
    {
        plantStageController.ResetStage();
        witherIcon.SetActive(true);
    }

    private void ReduceWater(int amount)
    {
        if (waterLevelCurrent > 0)
        {
            waterLevelCurrent -= amount;
            waterBarScript.SetWaterLevel(waterLevelCurrent);
            UpdateSoilMaterial();
        }
    }

    private void UpdateSoilMaterial()
    {
        float ratio = waterLevelCurrent / (float) waterLevelMax;
        Color newColor = Color.Lerp(soilMaterials[0].color, soilMaterials[1].color, ratio);

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColor);
        soilMound.setSoilMaterial(newColor);
    }
}
