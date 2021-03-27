using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public GameObject[] PLANT_STAGES = new GameObject[5];

    public const int WATER_LEVEL = 6;
    public const string SOIL_PREF = "CLAY_LOAM";
    
    private GameObject currentPrefab;
    private int currentIndex = 0;
    private PlantStageController controller;


    /**
     * API for PlantController to check if the soil and watering meets the conditions
     * for growth of plant
     */

    private void Start()
    {
        // Get PlantController
        controller = gameObject.GetComponentInParent<PlantStageController>();
    }

    public int GetPreferredWaterLevel()
    {
        return WATER_LEVEL;
    }

    public string GetSoilPref()
    {
        return SOIL_PREF;
    }

    public void NextStage()
    {
        if (currentIndex == 0)
        {
            // disable seed mesh
            GetComponent<MeshRenderer>().enabled = false;
        }

        SetPrefab(currentIndex + 1);
    }

    public bool IsNextStagePresent()
    {
        return currentIndex < PLANT_STAGES.Length;
    }

    private void SetPrefab(int index)
    {
        if (currentPrefab)
        {
            Destroy(currentPrefab);
        }

        currentPrefab = Instantiate(PLANT_STAGES[index], transform.position, Quaternion.identity, transform);
        currentIndex = index;
    }

    private void UpdateIsHarvestedState()
    {
        // calls the plant stage controller script
    }
}
