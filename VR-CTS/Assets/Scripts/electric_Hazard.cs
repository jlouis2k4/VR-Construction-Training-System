using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class electric_Hazard : MonoBehaviour
{
    private Hazard hazard;
    public ParticleSystem spark_Effect;
    public AudioClip spark_Sound;

    void Awake()
    {
        hazard = GetComponent<Hazard>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (other.gameObject.tag == "Player")
        {
            Destroy(spark_Effect);
            audio.clip = spark_Sound;
            audio.Pause();
            hazard.Completed = true; //communicates and updates score.
        }
    }
}
