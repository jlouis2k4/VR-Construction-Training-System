using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for elevator hazard interaction
/// </summary>
public class elevator_Hazard : MonoBehaviour
{
    private Hazard hazard;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        hazard = GetComponent<Hazard>();
    }

    /// <summary>
    /// Collider function: When an object with the tag 'barricade' collides with this object sets off a true command telling the game hazard has been completed
    /// </summary>
    /// <param name="other">The Collider of the other GameObject being collided with</param>
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "barricade")
        {
            //communicates that hazard is completed and updates score.
            hazard.Completed = true;

        }
      
    }

    /// <summary>
    /// Turns hazard completion to false, if barricade is removed
    /// </summary>
    /// <param name="other">The Collider of the other GameObject being collided with</param>
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            hazard.Completed = false;
        }
    }
}
