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
		GameObject other = collider.transform.gameObject;

		// If the seeds hit the collider, means there are seeds inside the hole
		// attach seeds onto the seedLocation
		if (other.CompareTag("Tomato_Seed"))
		{
			SM_Soil.SetHasSeed(true);
			SM_Soil.SetPlant("TOMATO");

			other.GetComponent<Rigidbody>().isKinematic = true;
			other.transform.position = seedLocation.transform.position;
			other.transform.parent = seedLocation.transform;
		}
	}
}
