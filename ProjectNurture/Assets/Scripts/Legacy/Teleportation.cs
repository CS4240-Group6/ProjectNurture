using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] OVRInput.Axis2D stick;
    [SerializeField] LineRenderer laser;
    [SerializeField] int laserSteps = 20;
    [SerializeField] float laserSegmentDistance = 1f, dropPerSegment = .1f, laserSize = .2f;
    [SerializeField] Transform head, cameraRig;
    [SerializeField] int collisionLayer;

    private Vector3 targetPos;

    bool targetAcquired = false;

    private void Awake()
    {
        laser.positionCount = laserSteps;
        laser.startWidth = laserSize;
        laser.endWidth = laserSize;
    }

    private void Update()
    {
        if (OVRInput.Get(stick).y > .8f) //if stick is pushed up
        {
            TryToGetTeleportTarget();
        }
        else if (targetAcquired == true && OVRInput.Get(stick).y < .2f)
        {
            Teleport();
        }
        else if (targetAcquired == false && OVRInput.Get(stick).y < .2f)
        {
            ResetLaser();
        }
    }

    private void TryToGetTeleportTarget()  //display line renderer and shoot out raycasts
    {
        targetAcquired = false;
        RaycastHit hit;
        Vector3 origin = transform.position;
        laser.SetPosition(0, origin);

        for (int i = 0; i < laserSteps - 1; i++)
        {
            Vector3 offset = (transform.forward + (Vector3.down * dropPerSegment * i)).normalized * laserSegmentDistance;

            if (Physics.Raycast(origin, offset, out hit, laserSegmentDistance)) //if we hit something
            {
                for (int j = i + 1; j < laser.positionCount; j++)
                {
                    laser.SetPosition(j, hit.point);
                }

                if (hit.transform.gameObject.layer == collisionLayer)  //if whatever we hit is a teleport point
                {
                    laser.startColor = laser.endColor = Color.green;
                    targetPos = hit.point;
                    targetAcquired = true;
                    return;
                }
                else //if whatever we hit is an invalid teleport point (i.e wall)
                {
                    laser.startColor = laser.endColor = Color.red;
                    return;
                }
            }
            else
            {
                laser.SetPosition(i + 1, origin + offset);
                origin += offset;
            }
        }

        laser.startColor = laser.endColor = Color.red;
    }

    private void Teleport()
    {
        targetAcquired = false;
        ResetLaser();

        Vector3 offset = new Vector3(targetPos.x - head.transform.position.x, targetPos.y - cameraRig.position.y, targetPos.z - head.transform.position.z);


        cameraRig.position += offset;
    }

    private void ResetLaser()  //reset line renderer
    {
        for (int i = 0; i < laser.positionCount; i++)
        {
            laser.SetPosition(i, Vector3.zero);
        }
    }
}