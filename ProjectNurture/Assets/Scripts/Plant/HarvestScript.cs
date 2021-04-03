using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HarvestScript : MonoBehaviour
{
    UnityEvent onGrab;
    UnityEvent onRelease;

    void Start()
    {
        onGrab = gameObject.GetComponent<Autohand.Grabbable>().onGrab;
        onGrab.AddListener(OnGrab);

        onRelease = gameObject.GetComponent<Autohand.Grabbable>().onRelease;
        onRelease.AddListener(OnRelease);
    }

    private void OnGrab()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnRelease()
    {
        this.gameObject.transform.parent = null;
    }

}
