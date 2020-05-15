using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains collision behavior for the worksite boss
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class start_Finish_Missions : MonoBehaviour
{
    public AudioClip Bosses_Voice;
    public AudioClip Bosses_Voice_Finish;
    public ExitMenu Exit_Menu;

    private int count = 0;

    /// <summary>
    /// Collider function that runs when this object's trigger Collider collides with another Collider.
    /// </summary>
    /// <param name="other">The Collider of the object that was collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AudioSource audio = GetComponent<AudioSource>();
            count++;
            if (count > 1)
            {
                audio.clip = Bosses_Voice_Finish;
                audio.Play();
                Exit_Menu.EnableConfirmationMenu();

            }
            else
            {
                audio.clip = Bosses_Voice;
                audio.Play();
            }
        }
    }
}
