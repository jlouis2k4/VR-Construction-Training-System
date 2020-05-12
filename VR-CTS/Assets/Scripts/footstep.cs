using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that plays footstep audio clips when the player moves
/// </summary>
public class footstep : MonoBehaviour
{

    CharacterController cc;
    public AudioClip otherClip;
    AudioSource audioSource;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (cc.isGrounded == true && cc.velocity.magnitude > 2f && audioSource.isPlaying == false)
        {
            audioSource.clip = otherClip;
            audioSource.volume = Random.Range(0.8f, 1);
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            audioSource.Play();
        }
    }
}