using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public GameObject scoreboard;

    private bool isProjectile = false;
    private AudioSource splatAudio;

    void Start()
    {
        splatAudio = GetComponent<AudioSource>();
        scoreboard = GameObject.Find("Scoring");
    }

    public void ToggleShotState()
    {
        // destroys itself if it has been present for too long without collision after it has been shot
        isProjectile = true;
        scoreboard.GetComponent<ScoreScript>().updateThrowCount();

    }

    // todo: bug: need to detect collision only after it has been shot. 
    // right now its detecting its spawned location (the plane) and immediately deleting itself
    void OnCollisionEnter(Collision col)
    {
        
        if (isProjectile)
        {
            // If the thing is a target, increment the score
            if (col.gameObject.CompareTag("Target"))
            {
                // Play splat if it's a scarecrow
                // Get audio from target
                AudioSource hayHitSound = col.gameObject.GetComponent<AudioSource>();
                AudioSource.PlayClipAtPoint(hayHitSound.clip, transform.position);

                scoreboard.GetComponent<ScoreScript>().updateScore();

                // destroy itself
                Destroy(gameObject);
                col.gameObject.SetActive(false);
            } 
        }
    }
}
