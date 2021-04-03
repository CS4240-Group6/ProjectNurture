using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedBagController : MonoBehaviour
{
    public GameObject seedPrefab;
    public GameObject spawnLocation;
    public LayerMask soilMask;
    public string playerTagLabel = "Player";

    private float raycastDistance = 1f;
    private float seedDropRate = 2f; // falls every 2 seconds
    private bool canSeedsDrop = true;

    void FixedUpdate()
    {
        RaycastHit hit;

        bool isBeingHeldByPlayer = transform.parent && transform.parent.tag == playerTagLabel;

        if (isBeingHeldByPlayer && canSeedsDrop && Physics.Raycast(this.transform.position, this.transform.up, out hit, raycastDistance, soilMask))
        {
            canSeedsDrop = false;
            StartCoroutine(DropSeed());
        } 

    }

    private IEnumerator DropSeed()
    {
        Vector3 seedPosition = spawnLocation.transform.position;
        Instantiate(seedPrefab, seedPosition, Quaternion.identity);
        yield return new WaitForSeconds(seedDropRate);
        canSeedsDrop = true;
    }
}
