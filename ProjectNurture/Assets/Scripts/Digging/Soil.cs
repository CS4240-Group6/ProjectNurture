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
	private string plant = "NO_PLANT";

	private GameObject ui_popup_seeds;
	private GameObject ui_popup_soil;

	public PlantStageController controller;

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
		this.gameObject.GetComponent<MeshCollider>().isTrigger = false;
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
}
