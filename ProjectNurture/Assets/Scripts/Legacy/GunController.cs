using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public TextController textController;
    public OVRInput.Controller Controller;

    public string actionButtonName;
    public string resetGameButtonName;
    public float grabRadius = 8.5f;
    public LayerMask grabMask;

    public float bulletLaunchForce = 5f; 
    public AudioClip grabFruitAudio;

    private GameObject projectile;
    private bool isArmed;
    private AudioSource audioSource;
    private GameObject scoreboard;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // restart level
        if (Input.GetAxis(resetGameButtonName) == 1)
        {
            Application.LoadLevel(0);
        }

        // either grab or shoot object
        if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown(actionButtonName))
        {
            //ShootTempObject();
            if (isArmed)
            {
                ShootObject();
            }
            else
            {
                GrabObject();
            }
        }
    }

    void ShootObject()
    {
        projectile.transform.parent = null;
        projectile.GetComponent<Rigidbody>().isKinematic = false;

        projectile.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * bulletLaunchForce, ForceMode.Impulse);
        projectile.GetComponent<ProjectileController>().ToggleShotState();

        isArmed = false;
        projectile = null;
    }

    void GrabObject()
    {        
        
        RaycastHit[] hits;

        // only react for objects in the correct layer(s)
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);

        if (hits.Length > 0)
        {
            int closestHit = 0;

            for (int i = 0; i < hits.Length; i++)
            {
                if ((hits[i]).distance < hits[closestHit].distance)
                {
                    closestHit = i;
                }
            }

            projectile = hits[closestHit].transform.gameObject; 
            projectile.GetComponent<Rigidbody>().isKinematic = true;

            Transform gunBarrel = transform.GetChild(0).transform; // get gun barrel transform

            projectile.transform.position = gunBarrel.position;
            projectile.transform.rotation = gunBarrel.rotation;
            projectile.transform.parent = transform;
            isArmed = true;

            // play audio
            audioSource.clip = grabFruitAudio;
            audioSource.Play();
        }
    }
}
