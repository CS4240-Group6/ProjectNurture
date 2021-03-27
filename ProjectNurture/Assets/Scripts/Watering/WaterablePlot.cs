using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterablePlot : MonoBehaviour
{
    public int waterLevelCurrent = 0;
    public int waterLevelMax = 10;
    public int waterLevelGoal = 7;
    public int waterLevelBuffer = 2; // if player waters more than goal + buffer, plant dies
    public float waterEvaporationPeriod = 60f; // water level is reduced by 1 every 60s

    public bool isCanvasVisible = true;
    public GameObject canvas = null;
    public WaterBar waterBar = null;
    public GameObject crossIcon = null;

    public AudioClip successSoundEffect = null;
    public AudioClip warningSoundEffect = null;

    // index 0 contains the dry soil material, index 1 contains the wet soil material
    public Material[] soilMaterials;

    private SoundController soundController = null;
    private PlantStageController plantStageController = null;
    private Soil soilMound;

    private void Awake()
    {
        soundController = GetComponent<SoundController>();
        plantStageController = GetComponent<PlantStageController>();
        soilMound = this.gameObject.transform.GetChild(1).GetComponent<Soil>(); // Get script of soil mound
    }


    private void Start()
    {
        canvas.SetActive(isCanvasVisible);
        
        waterBar.SetMaxWaterLevel(waterLevelMax, waterLevelGoal);
        waterBar.SetWaterLevel(waterLevelCurrent);

        StartCoroutine(ReduceWaterOverTime());
        UpdateSoilMaterial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetIsWaterCanvasVisible(!isCanvasVisible);
        }
    }

    public void SetIsWaterCanvasVisible(bool val)
    {
        isCanvasVisible = val;
        canvas.SetActive(val);
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
    public void ResetWater()
    {
        waterLevelCurrent = 0;
        waterBar.SetWaterLevel(waterLevelCurrent);
        UpdateSoilMaterial();
    }

    public void AddWater(int amount)
    {
        if (waterLevelCurrent != waterLevelMax)
        {
            waterLevelCurrent += amount;
            waterBar.SetWaterLevel(waterLevelCurrent);

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

    private void WaterSuccess()
    {
        soundController.PlayAudio(successSoundEffect);
        plantStageController.SetIsWatered(true);
    }

    private void WaterWarning()
    {
        soundController.PlayAudio(warningSoundEffect);
        crossIcon.SetActive(true);
    }
    private void WaterFail()
    {
        plantStageController.ResetStage();
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
        soilMound.setSoilMaterial(newColor);
    }
}
