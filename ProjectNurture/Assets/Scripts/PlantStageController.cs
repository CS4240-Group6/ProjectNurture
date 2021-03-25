using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStageController : MonoBehaviour
{
    public GameObject spawnLocation = null;
    public GameObject[] plantStages = null;
    public AudioClip newStageSoundEffect = null;
    public float newStageDelay = 3f;
    public int currentStageIndex = 0;
    public Vector3 modelScaleFactor = new Vector3(5f, 5f, 5f);

    private GameObject currentStage = null;
    private SoundController soundController = null;

    void Start()
    {
        soundController = GetComponent<SoundController>();
        SetCurrentStage(currentStageIndex, false);
    }

    public void NextStage()
    {
        if (currentStageIndex < plantStages.Length)
        {
            SetCurrentStage(currentStageIndex + 1, true);
        }
    }

    public void ResetStage()
    {
        SetCurrentStage(0, true);
    }

    private void SetCurrentStage(int index, bool shouldPlayAudio)
    {
        currentStageIndex = index;
        
        if (shouldPlayAudio) 
        {
            soundController.PlayAudio(newStageSoundEffect);
        }
        
        print(currentStage);
        if (currentStage != null)
        {
            Destroy(currentStage);   
        }

        print(currentStage);
        
        GameObject newStage = Instantiate(plantStages[index], spawnLocation.transform.position, Quaternion.identity, transform);
        newStage.transform.localScale = modelScaleFactor;
        currentStage = newStage;
    }

}
