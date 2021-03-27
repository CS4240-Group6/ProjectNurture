using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{

	public Mesh hole_0;
	public Mesh hole_50;
	public Mesh hole_100;

	// Variables for managing state of soil clod
	private float digging_state = 0;
	private GameObject plant;

	private GameObject ui_popup_seeds;
	private GameObject ui_popup_soil;
	private GameObject ui_popup_plant;

	public PlantStageController controller;
	private PlantScript plantScript;

	// Start is called before the first frame update
	void Start()
	{
		digging_state = 0;

		// Get PlantController from plot
		controller = this.gameObject.GetComponentInParent<PlantStageController>();

		// Get original mesh
		hole_0 = this.gameObject.GetComponent<MeshFilter>().mesh;

		// Set UI stuff to invisible first
		ui_popup_seeds = this.gameObject.transform.GetChild(0).gameObject;
		ui_popup_seeds.SetActive(false);

		ui_popup_soil = this.gameObject.transform.GetChild(1).gameObject;
		ui_popup_soil.SetActive(false);

        ui_popup_plant = this.gameObject.transform.GetChild(2).gameObject;
		ui_popup_plant.SetActive(false);
	}

	void Update()
	{
		// If there is a hole in the soil
		if (digging_state == 1.0f)
		{
			ui_popup_seeds.SetActive(true);

			if (controller.GetHasSeed())
			{
				ui_popup_seeds.SetActive(false);
				ui_popup_soil.SetActive(true);

				if (controller.GetIsSeedCovered())
				{
					ui_popup_soil.SetActive(false);
					ui_popup_plant.GetComponent<SpriteRenderer>().sprite = plantScript.GetUISprite();
					ui_popup_plant.SetActive(true);
					fillHole();
				}
			}
		}
	}

	public void ChangeSoilIntermediate()
	{
		digging_state = 0.5f;

		this.gameObject.GetComponent<MeshFilter>().mesh = hole_50;
	}

	public void ChangeSoilHole()
	{
		digging_state = 1.0f;

		this.gameObject.GetComponent<MeshFilter>().mesh = hole_100;
		this.gameObject.GetComponent<MeshCollider>().isTrigger = false;
	}

	public void fillHole()
	{
		digging_state = 0f;

		this.gameObject.GetComponent<MeshFilter>().mesh = hole_0;

		// Disable digging on the soil if the seed is inside and hole already covered
		this.gameObject.GetComponent<MeshCollider>().enabled = false;
	}

	/**
	 *	API for SoilCollider script
	 */

	public float getDiggingState()
	{
		return this.digging_state;
	}

	public void setSoilMaterial(Color newColor)
	{
		this.gameObject.GetComponent<Renderer>().material.SetColor("_Color", newColor);
	}

	public void SetHasSeed(bool boolean)
	{
		controller.SetHasSeed(boolean);
	}

	public void SetIsSeedCovered(bool boolean)
	{
		controller.SetIsSeedCovered(boolean);
	}

	public void SetPlant(PlantScript plantScript)
    {
		this.plantScript = plantScript;
		controller.SetCurrentPlant(plantScript);
    }

	public bool GetHasSeed()
    {
		return controller.GetHasSeed();
    }

}
