using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedCollider : MonoBehaviour
{
	public GameObject seedLocation;

	private Soil SM_Soil;   // Parent script
	private GameObject Parent;

	void Start()
    {
		SM_Soil = this.transform.parent.gameObject.GetComponent<Soil>();
		Parent = this.transform.parent.gameObject;
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



            //Move seed
            float step = 0.7f * Time.deltaTime;
            while (true) //While it hasn't reach the ground
            {
                Debug.Log("Moving");
                Vector3 newPos = Vector3.MoveTowards(other.transform.position, this.transform.position, step);
                other.transform.position = newPos;
                other.transform.parent.position = newPos;
            }



            // plant.transform.position = seedLocation.transform.position;
            // collider.transform.position = new Vector3(0, 0, 0);
            // plant.transform.parent = seedLocation.transform;
        }
	}
}
