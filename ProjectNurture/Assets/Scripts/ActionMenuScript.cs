using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuScript : MonoBehaviour
{
    // Debug 
    public TextController tempTextController;

    //1. Add public variable for canvas score counter UI
    private GameObject ScoreCounter;
    private GameObject ScorePanel;

    // Name of Action Menu game object in the scene
    public string actionMenuName = "ActionMenu";

    // Start is called before the first frame update
    void Start()
    {
        ScoreCounter = GameObject.Find(actionMenuName);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showUI()
    {
        ScoreCounter.SetActive(true);
    }

    public void removeUI()
    {
        ScoreCounter.SetActive(false);
    }

}

