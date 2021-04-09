using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularProgressBar : MonoBehaviour
{
    public Image LoadingBar;
    public Text ProgressIndicator;
    public Text ProgressText;

    float progress;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void updateProgress(float progress)
    {
        LoadingBar.fillAmount = progress;

        progress = progress * 100;
        ProgressIndicator.text = ((int)progress).ToString() + "%";

        if (progress >= 100)
            ProgressText.text = "Completed!";
    }
}
