using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that allows moving the player without VR for testing ourposes
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
   
    Vector3 velocity;
    bool isGrounded;

    /// <summary>
	/// Update is called once every frame.
	/// </summary>
    void Update()
    {
		if (GlobalData.PlayerCanMove) {

			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			Vector3 move = transform.right * x + transform.forward * z;

			controller.Move(move * speed * Time.deltaTime);

			velocity.y += gravity * Time.deltaTime;

			controller.Move(velocity * Time.deltaTime);
		}
    }
}
