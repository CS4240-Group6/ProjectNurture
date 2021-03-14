using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderController : MonoBehaviour
{
    public GameObject start; // assign which game obj it should start drawing from
    private LineRenderer lr;
    private Vector3[] points;
    private int defaultNumPoints = 2;
    private float defaultDistanceBetweenPoints = 5f;
    private float laserSize = .1f;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = laserSize;
        lr.endWidth = laserSize;
        lr.positionCount = defaultNumPoints;
        if (start == null)
        {
            start = gameObject;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 nextPosition = start.transform.position; // get gun barrel position
        for (int i = 0; i < defaultNumPoints; i++)
        {
            lr.SetPosition(i, nextPosition);
            nextPosition += defaultDistanceBetweenPoints * transform.forward;
        }
    }
}
