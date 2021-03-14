using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{

    // Debug 
    public TextController tempTextController;

    //1. Add public variable for canvas score counter UI
    private GameObject ScoreCounter;

    private GameObject ScorePanel;
    private Text ScarecrowScoreText;
    private Text FruitScoreText;

    private GameObject CircularProgressBar;
    private Image LoadingBar;
    private Image Center;
    private Text ProgressIndicator;
    private Text ProgressText;
   

    //2. Add private variable to keep track of:
    //How Many fruits thrown
    private int fruitThrown = 0;
    //How many scarecrows left
    public static int scarecrowLeft;
    //Constant of the total number of scarecrows
    public int TOTALSCARECROW = 10;

    // Name of scoreboard game object
    public string scoreboard_name = "Scoring";

    // Start is called before the first frame update
    void Start()
    {
        ScoreCounter = GameObject.Find(scoreboard_name);

        ScorePanel = ScoreCounter.transform.Find("Panel").gameObject;
        ScarecrowScoreText  = ScorePanel.transform.Find("Scarecrows Score").GetComponent<Text>();
        FruitScoreText = ScorePanel.transform.Find("Fruits Score").GetComponent<Text>();

        CircularProgressBar = ScorePanel.transform.Find("Circular Progress Bar").gameObject;


        scarecrowLeft = TOTALSCARECROW;

        /*
        LoadingBar = CircularProgressBar.transform.Find("Loading Bar").GetComponent<Image>();
        Center = CircularProgressBar.transform.Find("Center").GetComponent<Image>();
        ProgressIndicator = Center.transform.Find("Progress Indicator").GetComponent<Text>();
        ProgressText = Center.transform.Find("Progress Text").GetComponent<Text>();
        */
    }

    // Update is called once per frame
    void Update()
    {
        //3. Update variables here etc.
        /*
        if (scarecrowHit < TOTALSCARECROW)
        {
            fruitThrown++;
            scarecrowHit++;
        }*/

        /*
        //4. When button is clicked to show UI, Call update text method
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MOUSE BUTTON IS CLICKED");
            if (ScoreCounter.activeSelf)
            {
                Debug.Log("SCORE COUNTER IS ACTIVE, CALL REMOVEUI");
                removeUI();
            } else {
                Debug.Log("SCORE COUNTER IS NOT ACTIVE, CALL SHOWUI");
                showUI();
            }
                
        } */

        updateText();
        updateProgressBar();
    }

    
    public void showUI()
    {
        ScoreCounter.SetActive(true);
    }

    public void removeUI()
    {
        ScoreCounter.SetActive(false);
    }

    public void updateThrowCount()
    {
        fruitThrown++;
    }

    public void updateScore()
    {
        scarecrowLeft--;
    }

    void updateText()
    {
        FruitScoreText.text = fruitThrown.ToString();
        ScarecrowScoreText.text = scarecrowLeft.ToString();
    }

    void updateProgressBar()
    {
        float progress = (TOTALSCARECROW - scarecrowLeft) / (float)TOTALSCARECROW;

        CircularProgressBar.GetComponent<CircularProgressBar>().updateProgress(progress);
    }
}
