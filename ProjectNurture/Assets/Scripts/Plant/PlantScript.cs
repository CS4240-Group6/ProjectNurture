using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public GameObject[] PLANT_STAGES = new GameObject[5];

    public AudioClip nextStageSoundEffect;
    public AudioClip deadPlantSoundEffect;
    public AudioClip successSoundEffect;

    public const int WATER_LEVEL = 6;
    public const string SOIL_PREF = "CLAY_LOAM";

    public Material deadPlantMaterial;

    public Sprite ui_popup = null;

    private GameObject currentPrefab;
    private int currentIndex = 0;
    private bool isCurrentPlantBeingKilled = false;
    private float killPlantAnimationTotalTime = 3f;

    /**
     * API for PlantController to check if the soil and watering meets the conditions
     * for growth of plant
     */

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
        // prevent going to the next stage if the kill plant animation is ongoing
        if (!isCurrentPlantBeingKilled)
        {
            if (currentIndex == 0)
            {
                // disable seed mesh
                transform.GetChild(0).gameObject.SetActive(false);
            }

            PlayAudio(nextStageSoundEffect);
            SetPrefab(currentIndex + 1);
        }
    }

    public void KillPlant()
    {
        isCurrentPlantBeingKilled = true;
        if (currentIndex == 0)
        {
            PlayAudio(deadPlantSoundEffect);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(KillPlantAnimationWithColour());
        }
    }

    private IEnumerator KillPlantAnimationWithColour()
    {
        while (gameObject.activeSelf)
        {
            // change plant colour
            foreach (Transform child in currentPrefab.transform)
            {
                Renderer renderer = child.GetComponent<Renderer>();

                Material[] newMaterials = new Material[renderer.materials.Length];
                for (var j = 0; j < renderer.materials.Length; j++)
                {
                    newMaterials[j] = deadPlantMaterial;
                }
                renderer.materials = newMaterials;

            }

            // move plant downwards to imitate plant collapsing
            currentPrefab.GetComponent<Rigidbody>().isKinematic = false;

            PlayAudio(deadPlantSoundEffect);

            yield return new WaitForSeconds(killPlantAnimationTotalTime);
            Destroy(gameObject);
        }
    }

    public bool IsNextStagePresent()
    {
        return currentIndex < PLANT_STAGES.Length;
    }

    public bool IsHarvestStageComplete()
    {
        return false;
    }

    public bool IsPlantWaterable()
    {
        // plant cant be watered when it reaches the last stage
        return currentIndex < PLANT_STAGES.Length - 1;
    }

    public bool IsPlantWitherable()
    {
        // plant cant wither when it is a seed and when it reaches the last stage
        return currentIndex > 0 && currentIndex < PLANT_STAGES.Length - 1;
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

    private void PlayAudio(AudioClip c)
    {
        AudioSource.PlayClipAtPoint(c, transform.position);
    }

    public Sprite GetUISprite()
    {
        return ui_popup;
    }
}
