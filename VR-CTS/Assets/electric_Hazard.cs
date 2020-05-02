using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class electric_Hazard : MonoBehaviour
{
    public ParticleSystem spark_Effect;
    public AudioClip spark_Sound;
   private void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (other.gameObject.tag == "Player")
        {
            Destroy(spark_Effect);
            audio.clip = spark_Sound;
            audio.Pause();
        }
    }
}
