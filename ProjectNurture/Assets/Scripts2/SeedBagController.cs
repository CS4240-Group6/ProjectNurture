using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBagController : MonoBehaviour
{
    public GameObject target;
    public GameObject seedPrefab;
    public Text text;

    public float range = 10000;
    public int numSeeds = 3;

    private int soilMask = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //log
        text.text = "In update function";
        Debug.Log("In update function");

        RaycastHit hit;

        //log
        Debug.DrawRay(this.transform.position, this.transform.up * 500f, Color.green);
        Debug.Log(Physics.Raycast(this.transform.position, this.transform.up, out hit, 100, soilMask));
        Debug.Log("hit is " + hit.transform.name);

        if (Physics.Raycast(this.transform.position, this.transform.up, out hit, 100, soilMask))
        {
            //log
            Debug.Log("raycast has hit something!!!!");
            text.text = "Hit " + hit.transform.name;
            Debug.Log("Hit " + hit.transform.name);

            if (hit.transform.gameObject.CompareTag("SoilCollider"))
            {
                //log
                text.text = "Soil ccollider hit";
                Debug.Log("Soil ccollider hit");

                //log
                text.text = "Going to instantiate " + numSeeds;
                Debug.Log("Going to instantiate " + numSeeds);

                Vector3 seedPosition = this.transform.position + new Vector3(0, -0.29f, 0); //so that seed spawns from top of seed packet

                for (int i = 0; i < numSeeds; i++)
                {
                    Instantiate(seedPrefab, seedPosition, Quaternion.identity);
                }
            }
        }
    }
}
