using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStageController : MonoBehaviour
{
	public GameObject spawnLocation = null;
	public GameObject[] plantStages = null;
	public AudioClip newStageSoundEffect = null;
	public int currentStageIndex = 0;

	public float nextStageWaitDelay = 3f;
	public float witherTime = 20f; // player needs to reach the water level goal before this time

	public Material deadPlantMaterial = null;

	private GameObject currentStage = null;

	private SoundController soundController = null;
	private WaterablePlot waterablePlot = null;

	private Coroutine nextStageRoutine = null;
	private Coroutine witherTimerRoutine = null;

	private bool isWatered = false;
	private bool hasSeed = false;
	private bool isSeedCovered = false;

	private float destroyAnimationTime = 5f;
	private float witheredPlantDropSpeed = 5f;

	private int minWitherIndex = 1;
	private int maxWitherIndex = 4;

	private void Awake()
	{
		soundController = GetComponent<SoundController>();
		waterablePlot = GetComponent<WaterablePlot>();
	}

	private void Start()
	{
		SetCurrentStage(currentStageIndex, false);
	}

	private void Update()
	{
		if (hasSeed && isSeedCovered && isWatered && nextStageRoutine == null)
		{
			nextStageRoutine = StartCoroutine(StartNextStage());
		}
	}

	public void SetIsWatered(bool water)
	{
		isWatered = water;
	}

	public void SetHasSeed(bool seed)
	{
		hasSeed = seed;
	}

	public void SetIsSeedCovered(bool cover)
	{
		isSeedCovered = cover;
	}

	public bool GetHasSeed()
	{
		return hasSeed;
	}
	public bool GetIsSeedCovered()
	{
		return isSeedCovered;
	}

	// plant dies if it does not receive enough water in time
	private IEnumerator StartWitherTimer()
	{
		bool isTimerOngoing = true;
		while (isTimerOngoing)
		{
			yield return new WaitForSeconds(witherTime);

			ResetStage();
			isTimerOngoing = false;
		}
	}

	public void ResetStage()
	{
		if (nextStageRoutine != null)
		{
			StopCoroutine(nextStageRoutine);
			nextStageRoutine = null;
		}

		StartCoroutine(StartKillPlantAnimation());
	}

	private IEnumerator StartKillPlantAnimation()
	{
		bool isAnimationOngoing = true;
		while (isAnimationOngoing)
		{
			waterablePlot.ResetWater();
			isWatered = false;

			// if currentStage is rendered on the screen, change its colour and move it downwards
			Renderer renderer = currentStage.GetComponent<Renderer>();
			if (renderer != null)
			{
				Material[] newMaterials = new Material[renderer.materials.Length];
				for (var i = 0; i < renderer.materials.Length; i++)
                {
					newMaterials[i] = deadPlantMaterial;
                }
				renderer.materials = newMaterials;

				currentStage.GetComponent<Rigidbody>().velocity = new Vector3(0, -1) * witheredPlantDropSpeed;
			}

			yield return new WaitForSeconds(destroyAnimationTime);

			//if (renderer != null)
            //{
			//	SetCurrentStage(0, false);
			//} 
			//else 
			//{
				SetCurrentStage(0, true);
			//}

			hasSeed = false;
			isSeedCovered = false;
			isAnimationOngoing = false;
		}
	}

	private IEnumerator StartNextStage()
	{
		bool isTransitioningToNextStage = true;
		while (isTransitioningToNextStage)
		{
			yield return new WaitForSeconds(nextStageWaitDelay);

			NextStage();

			isTransitioningToNextStage = false;
			nextStageRoutine = null;
		}
	}

	private void NextStage()
	{
		if (currentStageIndex < plantStages.Length)
		{
			SetCurrentStage(currentStageIndex + 1, true);
		}
	}

	private void SetCurrentStage(int index, bool shouldPlayAudio)
	{
		Debug.Log("setting current stage to " + index);

		currentStageIndex = index;
		waterablePlot.ResetWater();
		isWatered = false;

		if (shouldPlayAudio) 
		{
			soundController.PlayAudio(newStageSoundEffect);
		}
		
		if (currentStage != null)
		{
			Destroy(currentStage);   
		}

		if (witherTimerRoutine != null)
		{
			StopCoroutine(witherTimerRoutine);
			witherTimerRoutine = null;
		}

		if (index >= minWitherIndex && index <= maxWitherIndex)
		{
			witherTimerRoutine = StartCoroutine(StartWitherTimer());
		}

		GameObject newStage = Instantiate(plantStages[index], spawnLocation.transform.position, Quaternion.identity, transform);
		currentStage = newStage;
	}

}
