using UnityEngine;

public class DebugEvent : MonoBehaviour
{
    private GameObject btn1, btn2, btn3;
    
    public void OnCustomButtonPress(string option)
    {
        Debug.Log("We pushed option " + option + "!");
        if (option == "1")
        {
            btn1 = GameObject.Find("Button1");
            btn1.GetComponent<AnswerScript>().Answer();  //activate the Answer() method in Button1 accordingly
            //Debug.Log("Answer method for btn1 activated");
        }
        else if (option == "2")
        {
            btn2 = GameObject.Find("Button2");
            btn2.GetComponent<AnswerScript>().Answer();  //activate the Answer() method in Button2 accordingly
            //Debug.Log("Answer method for btn2 activated");
        }
        else 
        {
            btn3 = GameObject.Find("Button3");
            btn3.GetComponent<AnswerScript>().Answer();  //activate the Answer() method in Button3 accordingly
            //Debug.Log("Answer method for btn3 activated");
        }
    }
}