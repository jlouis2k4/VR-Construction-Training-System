using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class start_Finish_Missions : MonoBehaviour
{
    public AudioClip Bosses_Voice;
    public AudioClip Bosses_Voice_Finish;
    public ExitMenu Exit_Menu;

    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
