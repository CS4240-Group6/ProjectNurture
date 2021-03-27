using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnButtonSelectScript : MonoBehaviour
{
    // Prefabs to spawn upon click
    public GameObject spadePrefab;
    public GameObject seedPrefab;
    public GameObject wateringCanPrefab;
    public GameObject harvestPrefab;
    public Transform rightHandTransform;

    List<GameObject> generatedObjects;

    // Start is called before the first frame update
    void Start()
    {
        generatedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to be called on button click
    public void SpawnTool(Button btn)
    {
        DestroyAll();

        GameObject clone;
        GameObject reference = btn.transform.GetChild(1).gameObject;
        
        if (btn != null) {
            if (btn.name == "dig_button")
            {
                Debug.Log("Dig button clicked");
                clone = Instantiate(spadePrefab, rightHandTransform.position + new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity);
                clone.transform.parent = GameObject.Find("RightHandAnchor").transform;
                clone.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                generatedObjects.Add(clone);
            }
            else if (btn.name == "plant_button")
            {
                Debug.Log("Plant button clicked");
                clone = Instantiate(seedPrefab, rightHandTransform.position + new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity);
                clone.transform.parent = GameObject.Find("RightHandAnchor").transform;
                clone.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                generatedObjects.Add(clone);
            }
            else if (btn.name == "water_button")
            {
                Debug.Log("Water button clicked");
                clone = Instantiate(wateringCanPrefab, rightHandTransform.position + new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity);
                clone.transform.parent = GameObject.Find("RightHandAnchor").transform;
                clone.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                generatedObjects.Add(clone);
            }
            else if (btn.name == "harvest_button")
            {
                Debug.Log("Harvest button clicked");
                clone = Instantiate(harvestPrefab, rightHandTransform.position + new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity);
                clone.transform.parent = GameObject.Find("RightHandAnchor").transform;
                clone.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                generatedObjects.Add(clone);
            }     
        }
    }

    public void DestroyAll() {

        foreach (GameObject clone in generatedObjects) 
        { 
            Destroy(clone);
        }
    }

}