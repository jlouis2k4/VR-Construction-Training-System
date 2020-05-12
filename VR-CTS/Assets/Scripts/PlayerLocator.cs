using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for moving the player to the position of the GameObject when the Virtual Reality level begins
/// </summary>
public class PlayerLocator : MonoBehaviour
{
    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    private void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
            GameObject.Destroy(gameObject);
        }
    }
}
