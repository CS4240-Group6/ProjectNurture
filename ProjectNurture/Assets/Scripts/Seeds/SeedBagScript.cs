using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBagScript : MonoBehaviour
{

    public GameObject seedPrefab;
    public Text textBox;

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {

    }

    private void OnTriggerExit(Collider collider)
    {
        textBox.text = "Left trigger";
        GameObject hand;

        textBox.text = "Creating seeds";

        // TO-DO: Need to check if collider is hand

        hand = collider.transform.parent.gameObject;
        for (int i = 0; i < 3; i++)
        {
            GameObject seed = Instantiate(seedPrefab, hand.transform.position, Quaternion.identity);
        }
        
    }
}
