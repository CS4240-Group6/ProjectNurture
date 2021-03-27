using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoScript : MonoBehaviour
{
    public GameObject[] TOMATO_STAGES = new GameObject[5];
    private const int WATER_LEVEL = 6;
    private const string SOIL_PREF = "CLAY_LOAM";

    /**
     * API for PlantController to check if the soil and watering meets the conditions
     * for growth of tomato
     */

    public int getWaterLevel()
    {
        return WATER_LEVEL;
    }

    public string getSoilPref()
    {
        return SOIL_PREF;
    }

    public void SpawnNextStage()
    {

    }
}
