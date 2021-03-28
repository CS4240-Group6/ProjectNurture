using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBagController : MonoBehaviour
{
    public GameObject target;
    public GameObject seedPrefab;
    //public Text text;

    public float range = 10000;
    public int numSeeds = 3;

    private int soilMask = 1 << 6;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("FrameUpdate", 0.0f, 0.7f);
  
    }

    // Update is called once per frame
    void FrameUpdate()
    {
        //log
      //  text.text = "In update function";
        Debug.Log("In update function");

        RaycastHit hit;

        //log
        Debug.DrawRay(this.transform.position, this.transform.up * 500f, Color.red);
        Debug.Log(Physics.Raycast(this.transform.position, this.transform.up, out hit, 100, soilMask));
        Debug.Log("hit is " + hit.transform.name);

        if (Physics.Raycast(this.transform.position, this.transform.up, out hit, 100, soilMask))
        {

            Vector3 seedPosition = this.transform.position + this.transform.up * 0.29f;
       
                Instantiate(seedPrefab, seedPosition, Quaternion.identity);
   
        }


    }
}

