using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script of player for testing assets and objects
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
   

    Vector3 velocity;
    bool isGrounded;
    // Update is called once per frame
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
