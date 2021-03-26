using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ensures that the UI always faces the player
public class BillboardScript : MonoBehaviour
{

    public GameObject camera;

    void LateUpdate()
    {
        Quaternion newRotation = Quaternion.LookRotation(camera.transform.position, camera.transform.up);
        transform.rotation = newRotation;
        // transform.LookAt(camera.transform);  * new Vector3(1f, 0, 1f);
    }
}
