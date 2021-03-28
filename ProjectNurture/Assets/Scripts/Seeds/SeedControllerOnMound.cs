using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedControllerOnMound : MonoBehaviour
{
    private Soil SM_Soil;
    // Start is called before the first frame update
    void Start()
    {
        SM_Soil = this.GetComponent<Soil>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("Seed"))
        {


            // GameObject parent = collider.transform.parent.gameObject;
            Debug.Log("Seed has touched the ground " + collision.transform.name);
            GameObject other = collision.transform.gameObject;


            if (SM_Soil.GetHasSeed())
            {
              
                Destroy(other);
                Debug.Log("Object is destroyed: " + other);
            }
            else
            {

                PlantScript plantScript = other.GetComponent<PlantScript>();
                Debug.Log("found plant script" + plantScript);

                SM_Soil.SetHasSeed(true);
                SM_Soil.SetPlant(plantScript);
            }


        }
    }
}
