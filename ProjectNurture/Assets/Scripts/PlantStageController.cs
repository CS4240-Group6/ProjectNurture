using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStageController : MonoBehaviour
{
	public GameObject spawnLocation = null;
	public GameObject[] plantStages = null;
	public AudioClip newStageSoundEffect = null;
	public int currentStageIndex = 0;

	public float nextStageWaitDelay = 10f;

	public Material deadPlantMaterial = null;

	private GameObject currentStage = null;

	private SoundController soundController = null;
	private WaterablePlot waterablePlot = null;

	private Coroutine nextStageRoutine = null;
	private Coroutine witherTimerRoutine = null;

	private bool isWatered = false;
	private bool hasSeed = true;
	private bool isSeedCovered = true;

	private float destroyAnimationTime = 5f;
	private float witherTime = 10f;

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
			Debug.Log("starting coroutine");
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

	public void ResetStage()
	{
		StartCoroutine(StartKillPlantAnimation());

		Debug.Log("stopping next stage coroutine");

		if (nextStageRoutine != null)
		{
			StopCoroutine(nextStageRoutine);
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

	private IEnumerator StartNextStage()
	{
		bool isTransitioningToNextStage = true;
		while (isTransitioningToNextStage)
		{
			Debug.Log("coroutine started");
			yield return new WaitForSeconds(nextStageWaitDelay);

			Debug.Log("coroutine triggers next stage");
			NextStage();

			isTransitioningToNextStage = false;
			nextStageRoutine = null;
		}
	}

	private IEnumerator StartKillPlantAnimation()
	{
		bool isAnimationOngoing = true;
		while (isAnimationOngoing)
		{
			Material[] materials = currentStage.GetComponent<MeshRenderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				currentStage.GetComponent<MeshRenderer>().materials[i] = deadPlantMaterial;
			}

			currentStage.GetComponent<Rigidbody>().useGravity = true;
			yield return new WaitForSeconds(destroyAnimationTime);
			SetCurrentStage(0, true);

			hasSeed = false;
			isSeedCovered = false;
			isAnimationOngoing = false;
		}
	}

	// plant dies if it does not receive enough water in time
	private IEnumerator StartWitherTimer()
	{
		Debug.Log("wither coroutine started");
		bool isTimerOngoing = true;
		while (isTimerOngoing)
		{
			yield return new WaitForSeconds(witherTime);

			Debug.Log("resetting...");

			ResetStage();
			isTimerOngoing = false;
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

		if (index >= minWitherIndex || index <= maxWitherIndex)
		{
			witherTimerRoutine = StartCoroutine(StartWitherTimer());
		}

		GameObject newStage = Instantiate(plantStages[index], spawnLocation.transform.position, Quaternion.identity, transform);
		currentStage = newStage;
	}

}
