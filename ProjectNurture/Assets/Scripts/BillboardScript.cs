using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ensures that the UI always faces the player
public class BillboardScript : MonoBehaviour
{
    public Transform cam;

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
        
    }
}
