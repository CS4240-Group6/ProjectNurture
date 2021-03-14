using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    /**
     * Script for toggling scoreboard on right hand
     */
    
    public string showScoreButtonName = "ShowScore";
    
    // Scoreboard game object public so that can manually change if needed    
    public GameObject scoreboard;

    // Start is called before the first frame update
    void Start()
    {
        scoreboard = GameObject.Find("Scoring");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(showScoreButtonName) == 1)
        {
            scoreboard.GetComponent<ScoreScript>().showUI();
        } else
        {
            scoreboard.GetComponent<ScoreScript>().removeUI();
        }
    }
}
