using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

		if (collider.transform.gameObject.CompareTag("Seed"))
		{

            // GameObject parent = collider.transform.parent.gameObject;
            Debug.Log("collided with " + collider.transform.name);
			GameObject other = collider.transform.gameObject;

			PlantScript plantScript = other.GetComponent<PlantScript>();
			Debug.Log("found plant script" + plantScript);

			SM_Soil.SetHasSeed(true);
			SM_Soil.SetPlant(plantScript);

			other.GetComponent<Rigidbody>().isKinematic = true;
			other.transform.position = seedLocation.transform.position;
			other.transform.parent = seedLocation.transform;


			// plant.transform.position = seedLocation.transform.position;
			// collider.transform.position = new Vector3(0, 0, 0);
			// plant.transform.parent = seedLocation.transform;
		}
	}
}
