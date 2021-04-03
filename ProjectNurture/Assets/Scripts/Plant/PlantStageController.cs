using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStageController : MonoBehaviour
{
	private PlantScript plantScript;
	private WaterablePlot waterablePlot = null;

	// plant state
	private bool isWatered = false;
	private bool hasSeed = false;
	private bool isSeedCovered = false;
	private bool isPlotWaterable = false;

	// coroutine that changes plants stage when timer reaches 0
	private Coroutine nextStageRoutine = null;
	private Coroutine witherTimerRoutine = null;

	// time for coroutines
	public float nextStageWaitDelay = 3f;
	public float witherTime = 20f;
	public float resetTimeDelay = 1.5f;

	public AudioClip failSoundEffect;
	public AudioClip successSoundEffect;
	public AudioClip nextStageSoundEffect;

	private void Awake()
	{
		waterablePlot = GetComponent<WaterablePlot>();
	}

	private void Update()
	{
		// to update water bar ui
		bool isCurPlotWaterable = IsPlotWaterable();
		if (isCurPlotWaterable != isPlotWaterable)
        {
			waterablePlot.SetIsPlotWaterable(isCurPlotWaterable);
			isPlotWaterable = isCurPlotWaterable;
		}

		// to advance to the next stage
		if (IsPlantNeedsMet() && plantScript && plantScript.IsNextStagePresent() && nextStageRoutine == null)
		{
			// start the countdown to advance to the next stage
			nextStageRoutine = StartCoroutine(StartNextStage());
		}

		// to reset the field once all fruits are harvested
		else if (plantScript && plantScript.IsHarvestStageComplete())
		{
			ResetStageInSuccess();
		}

        // to kill the plant if the soil is not correct
        else if (hasSeed && isSeedCovered && plantScript && !IsSoilTypeCorrect())
        {
            ResetStageInFailure();
            waterablePlot.DisplayWrongSoilUI();
        }
    }

	private bool IsPlotWaterable()
    {
		// plot can only be watered if it is covered and the plant can be watered
		return hasSeed && isSeedCovered && plantScript && plantScript.IsPlantWaterable();
    }

	private bool IsPlantNeedsMet()
    {
		return hasSeed && isSeedCovered && isWatered;
    }

	private bool IsSoilTypeCorrect()
    {
		return plantScript.GetSoilPref() == waterablePlot.GetSoilType();
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

	public void SetCurrentPlant(PlantScript plant)
    {
		plantScript = plant;
		waterablePlot.SetWaterGoal(plantScript.GetPreferredWaterLevel());
	}

	public bool GetHasSeed()
	{
		return hasSeed;
	}
	public bool GetIsSeedCovered()
	{
		return isSeedCovered;
	}

	public void ResetStageInFailure()
    {
		// play fail audio
		PlayAudio(failSoundEffect);

		ResetStage();
	}

	public void ResetStageInSuccess()
	{
		// play success audio
		PlayAudio(successSoundEffect);

		ResetStage();
	}

	private void ResetStage()
    {
		if (nextStageRoutine != null)
		{
			StopCoroutine(nextStageRoutine);
			nextStageRoutine = null;
		}

		// call plantScript to destroy itself
		plantScript.KillPlant();
		plantScript = null;

		// reset plant state after a delay
		StartCoroutine(StartReset());
	}

	private IEnumerator StartReset()
    {
		yield return new WaitForSeconds(resetTimeDelay);
		// reset water
		waterablePlot.ResetWater();
		isWatered = false;

		// reset seed state
		hasSeed = false;
		isSeedCovered = false;
	}

	private IEnumerator StartNextStage()
	{
		bool isTransitioningToNextStage = true;
		while (isTransitioningToNextStage)
		{
			// since needs are met, stop the previous stage's wither timer if it exists
			if (witherTimerRoutine != null)
			{
				StopCoroutine(witherTimerRoutine);
			}

			yield return new WaitForSeconds(nextStageWaitDelay);

			// play next stage audio
			PlayAudio(nextStageSoundEffect);

			// go to the next stage
			plantScript.NextStage();

			// reset water
			waterablePlot.ResetWater();
			isWatered = false;

			// start a new wither timer for this stage
			bool shouldStartWitherTimer = plantScript.IsPlantWitherable();
			if (shouldStartWitherTimer)
            {
				witherTimerRoutine = StartCoroutine(StartWitherTimer());
            }

			// reset next stage routine
			isTransitioningToNextStage = false;
			nextStageRoutine = null;
		}
	}

	// kill the plant if it has not reached its water goal when the timer reaches 0
	private IEnumerator StartWitherTimer()
	{
		bool isTimerOngoing = true;
		while (isTimerOngoing)
		{
			yield return new WaitForSeconds(witherTime);

			ResetStageInFailure();
			isTimerOngoing = false;
		}
	}

	private void PlayAudio(AudioClip c)
	{
		AudioSource.PlayClipAtPoint(c, transform.position);
	}
}
