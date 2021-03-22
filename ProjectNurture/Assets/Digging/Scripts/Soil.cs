using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{

	public Mesh hole_50;
	public Mesh hole_100;

	private float digging_state = 0;
	private GameObject ui_popup;


	// Start is called before the first frame update
	void Start()
	{
		ui_popup = this.gameObject.transform.GetChild(0).gameObject;
		ui_popup.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		// If there is a hole in the soil
		if (digging_state == 1.0f)
		{
			ui_popup.SetActive(true);
		}
	}

	void ChangeSoilIntermediate()
    {
		digging_state = 0.5f;

		this.gameObject.GetComponent<MeshFilter>().mesh = hole_50;
		
		// Reset mesh collider
		DestroyImmediate(this.gameObject.GetComponent<MeshCollider>());
		var collider = this.gameObject.AddComponent<MeshCollider>();
		collider.sharedMesh = hole_50;
    }

	void ChangeSoilHole()
    {
		digging_state = 1.0f;

		this.gameObject.GetComponent<MeshFilter>().mesh = hole_100;

		//Reset mesh collider
		DestroyImmediate(this.gameObject.GetComponent<MeshCollider>());
		var collider = this.gameObject.AddComponent<MeshCollider>();
		collider.sharedMesh = hole_100;
	}


	private void OnCollisionExit(Collision collision)
	{

		if (digging_state == 0f)
		{
			ChangeSoilIntermediate();
		}


		if (digging_state == 0.5f)
		{
			ChangeSoilHole();
		}
		
	}

}
