using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterStream : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    private ParticleSystem splashParticle = null;

    private Coroutine pourRoutine = null;
    private Vector3 targetPosition = Vector3.zero;

    private float raycastDistance = 10f;
    private float streamSpeed = 1.75f;

    private WaterablePlot currentPlot = null;
    private float waterSpeed = 1f; // waters every 1s
    
    private void Awake()
    {  
        lineRenderer = GetComponent<LineRenderer>();
        splashParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        MoveToPosition(0, transform.position);
        MoveToPosition(1, transform.position);
    }

    public void Begin()
    {
        StartCoroutine(UpdateParticle());
        pourRoutine = StartCoroutine(BeginPour());
        StartCoroutine(WaterPlot());
    }

    public void End()
    {
        StopCoroutine(pourRoutine);
        pourRoutine = StartCoroutine(EndPour());
    }

    private IEnumerator BeginPour()
    {
        while(gameObject.activeSelf)
        {
            targetPosition = FindEndPoint();

            MoveToPosition(0, transform.position);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }
    }

    private IEnumerator EndPour()
    {
        while(!HasReachedPosition(0, targetPosition))
        {
            AnimateToPosition(0, targetPosition);
            AnimateToPosition(1, targetPosition);

            yield return null;
        }

        // destroy once the stream has reached the target position, like the ground
        Destroy(gameObject);
    }

    private IEnumerator WaterPlot()
    {
        while(!HasReachedPosition(0, targetPosition))
        {
            bool isHitting = HasReachedPosition(1, targetPosition);
            // increment water if the stream has collided onto something, and currentPlot exists 
            if (isHitting && currentPlot != null)
            {
                currentPlot.AddWater(1);
            }

            yield return new WaitForSeconds(waterSpeed);
        }
    }

    private IEnumerator UpdateParticle()
    {
        while (gameObject.activeSelf)
        {
            splashParticle.gameObject.transform.position = targetPosition;

            bool isHitting = HasReachedPosition(1, targetPosition);
            splashParticle.gameObject.SetActive(isHitting);

            yield return null;
        }
    }

    private Vector3 FindEndPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        
        // search downwards for a particular distance
        Physics.Raycast(ray, out hit, raycastDistance); 

        // get the position of the collision location or the end of the raycast
        Vector3 endPoint;
        if (hit.collider)
        {
            endPoint = hit.point;

            // assign currentPlot if the stream is going to hit a plot
            if (hit.collider.gameObject.CompareTag("Waterable"))
            {
                currentPlot = hit.collider.GetComponent<WaterablePlot>();
            } 
            else 
            {
                currentPlot = null;
            }
        }
        else 
        {
            endPoint = ray.GetPoint(raycastDistance);
            currentPlot = null;
        }

        return endPoint;
    }

    private void MoveToPosition(int index, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(index, targetPosition);
    }

    private void AnimateToPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(index);
        Vector3 newPosition = Vector3.MoveTowards(currentPoint, targetPosition, Time.deltaTime * streamSpeed);
        lineRenderer.SetPosition(index, newPosition);
    }

    private bool HasReachedPosition(int index, Vector3 targetPosition)
    {
        Vector3 currentPosition = lineRenderer.GetPosition(index);
        return currentPosition == targetPosition;
    }

}
