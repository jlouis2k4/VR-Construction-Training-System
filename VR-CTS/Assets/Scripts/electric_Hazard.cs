using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for electric hazard interaction
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class electric_Hazard : MonoBehaviour
{
    private Hazard hazard;
    public ParticleSystem spark_Effect;
    public AudioClip spark_Sound;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        hazard = GetComponent<Hazard>();
    }

    /// <summary>
    /// Collider function that runs when this object's trigger Collider collides with another Collider.
    /// </summary>
    /// <param name="other">The Collider of the object that was collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (other.gameObject.tag == "Player")
        {
            Destroy(spark_Effect);
            audio.clip = spark_Sound;
            audio.Pause();
            //communicates that hazard is completed and updates score.
            hazard.Completed = true;
        }
    }
}
