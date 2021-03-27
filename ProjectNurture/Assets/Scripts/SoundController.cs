using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    private AudioSource audioPlayer = null;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();

    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            audioPlayer.clip = audioClip;
            audioPlayer.Play();
        }
    }
}
