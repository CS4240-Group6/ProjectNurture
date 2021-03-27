using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HarvestScript : MonoBehaviour
{
    UnityEvent onGrab;

    // Start is called before the first frame update
    void Start()
    {
        onGrab = gameObject.GetComponent<Autohand.Grabbable>().onGrab;
        onGrab.AddListener(OnGrab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGrab()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

}
