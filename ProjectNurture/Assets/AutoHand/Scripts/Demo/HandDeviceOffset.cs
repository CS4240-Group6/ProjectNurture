using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandDeviceOffset : MonoBehaviour
{
    public Transform offset;
    [SerializeField]
    public DeviceOffset[] deviceOffsets;
    
    List<InputDevice> devices = new List<InputDevice>();
    
    void Start(){
        InputDevices.GetDevices(devices);
        foreach(var device in devices) {
            Debug.Log(device.manufacturer);
            Debug.Log(device.name);
            Debug.Log("-------------");
        }

        foreach(var device in deviceOffsets) {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public struct DeviceOffset {
    public string deviceName;
    public Vector3 posOffset;
    public Vector3 rotOffset;
}
