using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoilCollider : MonoBehaviour
{
	// Debug
	public Text debug_text;

	// Variables for soil audio
	public AudioClip dig_enter;
	public AudioClip dig_leave;

	private AudioSource dig_enter_source;
	private AudioSource dig_leave_source;

	private Soil SM_Soil;   // Parent script

	// Start is called before the first frame update
	void Start()
	{
		// Get parent script
		SM_Soil = this.transform.parent.gameObject.GetComponent<Soil>();

		dig_enter_source = this.gameObject.AddComponent<AudioSource>();
		dig_leave_source = this.gameObject.AddComponent<AudioSource>();

		dig_enter_source.clip = dig_enter;
		dig_leave_source.clip = dig_leave;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey("s"))
        {
			SM_Soil.setHasSeeds(true);
        }

		if (Input.GetKey("d"))
        {
			SM_Soil.setHasSoil(true);
        }
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.transform.gameObject.CompareTag("Spade"))
		{
			debug_text.text = "Spade entering collider";
			dig_enter_source.Play();
		}
		
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.transform.gameObject.CompareTag("Spade"))
		{
			debug_text.text = "Spade leaving collider";

			// TODO: Debug why unable to transition to intermediate state
			if (SM_Soil.getDiggingState() == 0f)
			{
				dig_leave_source.Play();
				debug_text.text = "Soil level from 0f going to 0.5f";
				SM_Soil.ChangeSoilIntermediate();
				return;
			}


			if (SM_Soil.getDiggingState() == 0.5f)
			{
				dig_leave_source.Play();
				debug_text.text = "Soil level from 0.5f going to 1f";
				SM_Soil.ChangeSoilHole();
				return;
			}

			return;
		}

		// If the seeds hit the collider, means there are seeds inside the hole
		if (collider.transform.gameObject.CompareTag("Seed"))
        {
			SM_Soil.setHasSeeds(true);
        }
	}
}
