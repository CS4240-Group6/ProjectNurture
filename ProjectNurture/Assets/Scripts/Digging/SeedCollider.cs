using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCollider : MonoBehaviour
{
	public GameObject seedLocation;

	private Soil SM_Soil;   // Parent script

	void Start()
    {
		SM_Soil = this.transform.parent.gameObject.GetComponent<Soil>();
	}

	private void OnTriggerEnter(Collider collider)
	{

		// If the seeds hit the collider, means there are seeds inside the hole
		// attach seeds onto the seedLocation
		Debug.Log("entered seed collider trigger");

		if (collider.transform.gameObject.CompareTag("Seed"))
		{
			Debug.Log("seed detected");

			collider.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			collider.transform.position = seedLocation.transform.position;

			GameObject plant = collider.transform.parent.gameObject;

			SM_Soil.SetHasSeed(true);
			SM_Soil.SetPlant(plant);

			plant.transform.position = seedLocation.transform.position;
			plant.transform.parent = seedLocation.transform;
		}
	}
}
