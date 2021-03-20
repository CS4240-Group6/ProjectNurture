using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineRendererSettings : MonoBehaviour
{
    // Declare a LineRenderer to store the component attached to the GameObject.
    [SerializeField] LineRenderer rend;

    // Settings for the LineRenderer are stored as a Vector3 array of points. 
    // Set up a V3 array to //initialize in Start
    Vector3[] points;

    public LayerMask layerMask;

    // Declare the panel to change upon click
    public Button btn;
    public Image img;

    // Start is called before the first frame update
    void Start()
    {
        //get the LineRenderer attached to the game object.
        rend = gameObject.GetComponent<LineRenderer>();

        //initialize the LineRenderer
        points = new Vector3[2];

        //set the start point of the line renderer to the position of the gameObject.
        points[0] = Vector3.zero;

        //set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = transform.position + new Vector3(0, 0, 20);

        //finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);
        rend.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        AlignLineRenderer(rend);
    }

    public void AlignLineRenderer(LineRenderer rend) 
    {
        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            btn = hit.collider.gameObject.GetComponent<Button>();
            points[1] = transform.forward + new Vector3(0, 0, hit.distance);
            rend.startColor = Color.red;
            rend.endColor = Color.red;
        }
        else 
        {
            points[1] = transform.forward + new Vector3(0, 0, 20);
            rend.startColor = Color.green;
            rend.endColor = Color.green;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
    }

    public void ColorChangeOnClick() 
    {
        if (btn != null) 
        {
            img = btn.GetComponent<Image>();
            if (btn.name == "dig_button")
            {
                img.color = Color.gray;
                Debug.Log("Dig button clicked");
            }
            else if (btn.name == "plant_button")
            {
                img.color = Color.green;
                Debug.Log("Plant button clicked");
            }
            else if (btn.name == "water_button")
            {
                img.color = Color.blue;
                Debug.Log("Water button clicked");
            }
            else if (btn.name == "harvest_button") 
            {
                img.color = Color.yellow;
                Debug.Log("Harvest button clicked");
            }
        }
    
    }
}