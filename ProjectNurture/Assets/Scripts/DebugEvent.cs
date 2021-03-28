using UnityEngine;

public class DebugEvent : MonoBehaviour
{
    public void OnCustomButtonPress(string option)
    {
        Debug.Log("We pushed option " + option + "!");
    }
}