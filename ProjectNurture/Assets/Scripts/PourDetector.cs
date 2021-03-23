using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform spout = null;
    public GameObject streamPrefab = null;

    private bool isPouring = false;
    private WaterStream currentStream = null;

    void Update()
    {
        bool shouldPour = CalculatePourAngle() < pourThreshold;

        if (isPouring != shouldPour)
        {
            isPouring = shouldPour;

            if (isPouring)
            {
                StartPour();
            }
            else
            {
                EndPour();
            }
        }
    }

    private void StartPour()
    {
        Debug.Log("start pouring");

        currentStream = CreateStream();
        currentStream.Begin();
    }

    private void EndPour()
    {
        Debug.Log("end pouring");

        currentStream.End();
        currentStream = null;
    }

    private float CalculatePourAngle()
    {
        // return 0;
        return transform.forward.y * Mathf.Rad2Deg;
    }

    private WaterStream CreateStream()
    {
        GameObject streamObject = Instantiate(streamPrefab, spout.position, Quaternion.identity, transform);
        return streamObject.GetComponent<WaterStream>();
    }

}
