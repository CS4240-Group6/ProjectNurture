using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionSound : MonoBehaviour{
    [Tooltip("The layers that cause the sound to play")]
    public LayerMask collisionTriggers = ~0;
    [Tooltip("Source to play sound from")]
    public AudioSource source;
    [Tooltip("Source to play sound from")]
    public AudioClip clip;
    [Space]
    [Tooltip("Source to play sound from")]
    public AnimationCurve velocityVolumeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    float volumeAmp = 0.8f;
    float velocityAmp = 0.5f;
    float soundDelay = 0.1f;
    
    Rigidbody body;
    bool canPlaySound = true;

    private void Start() {
        body = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision) {
        if(canPlaySound && collisionTriggers == (collisionTriggers | (1 << collision.gameObject.layer))) {
            if(clip != null){
                source.PlayOneShot(clip, velocityVolumeCurve.Evaluate(body.velocity.magnitude*velocityAmp)*volumeAmp);
                StartCoroutine(SoundPlayBuffer());
            }
        }
    }

    IEnumerator SoundPlayBuffer() {
        canPlaySound = false;
        yield return new WaitForSeconds(soundDelay);
        canPlaySound = true;
    }
}
