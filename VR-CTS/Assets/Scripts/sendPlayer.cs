using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that moves the Player if they collide with the attached GameObject's Collider
/// </summary>
public class sendPlayer : MonoBehaviour
{
    public Transform respawnPoint = null;

    /// <summary>
    /// Collider function that runs when this object's trigger Collider collides with another Collider.
    /// </summary>
    /// <param name="other">The Collider of the object that was collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        print("player touched!\n");
        if (other.gameObject.tag == "Player")
        {
            print("player die!!\n");
            other.transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y, respawnPoint.position.z);
        }
    }
}
