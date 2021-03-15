using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DemoSceneHelper : MonoBehaviour{
    List<InputDevice> devices = new List<InputDevice>();
    
    void Update(){
        InputDevices.GetDevices(devices);
        foreach(var device in devices) {
            Debug.Log(device.manufacturer);
            Debug.Log(device.name);
            Debug.Log("-------------");
        }
    }
}
