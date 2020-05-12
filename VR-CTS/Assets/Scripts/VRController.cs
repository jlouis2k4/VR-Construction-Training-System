using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Class that controls the movement of the player character in VR based on the position
/// of the VR headset and input from the VR Controllers.
/// </summary>
public class VRController : MonoBehaviour 
{
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_Gravity = 30.0f;
    public float m_RotateIncrement = 90;

    public SteamVR_Action_Boolean m_RotatePress1 = null;
    public SteamVR_Action_Boolean m_RotatePress2 = null;
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_CharacterController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    private void Awake() 
    {
        m_CharacterController = GetComponent<CharacterController>();

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled, before the first Update().
    /// </summary>
    void Start() 
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    /// <summary>
    /// Update is called once every frame.
    /// </summary>
    void Update() 
    {
        HandleHeight();
        CalculateMovement();
        fluidMovement();
    }

    /// <summary>
    /// Sets the height of the character controller and collider based on the position of the user's head.
    /// </summary>
    private void HandleHeight() 
    {
        //Get the head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharacterController.height = headHeight;

        //Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharacterController.height / 2;
        newCenter.y += m_CharacterController.skinWidth;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        //Apply
        m_CharacterController.center = newCenter;
    }

    /// <summary>
    /// Moves the player character based on the the rotation of the VR headset and any movement input from the VR Controllers.
    /// </summary>
    private void CalculateMovement() 
    {
        //Figure out movement orientation

        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        //If not moving
        if (m_MoveValue.axis.magnitude == 0) 
        {
            m_Speed = 0;
        }

        // Add, clamp
        m_Speed += m_MoveValue.axis.magnitude * m_Sensitivity;
        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

        // Orientation and Gravity
        movement += orientation * (m_Speed * Vector3.forward);
        movement.y -= m_Gravity * Time.deltaTime;

        //Apply
        m_CharacterController.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// Returns the rotation that the player character should have to match the VR headset.
    /// </summary>
    /// <returns>A quaternion containing the new rotation of the player character.</returns>
    private Quaternion CalculateOrientation() 
    {
        float rotation = Mathf.Atan2(m_MoveValue.axis.x, m_MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);

    }

    /// <summary>
    /// Lerp's player character rotation for smooth movement.
    /// </summary>
    private void fluidMovement() 
    {
        float snapValue = 0.0f;

        if (m_RotatePress1.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = -Mathf.Abs(m_RotateIncrement);

        if (m_RotatePress2.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = Mathf.Abs(m_RotateIncrement);

        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }
}
