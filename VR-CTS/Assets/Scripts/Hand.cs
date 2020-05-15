using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Class handling the interaction between the Virtual Reality 'hands' and objects in the level
/// </summary>
public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    private List<Interactable> m_ContactInteractables = new List<Interactable>();

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    /// <summary>
    /// Update is called once every frame.
    /// </summary>
    void Update()
    {
        if (m_GrabAction.GetStateDown(m_Pose.inputSource)) { }
        Pickup();

        if (m_GrabAction.GetStateUp(m_Pose.inputSource)) { }
        Drop();
    }

    /// <summary>
    /// Collider function: Called when another Collider intersects this GameObject's trigger Collider. If the other object is tagged as Interactable, add it to the list of objects that can be interacted with currently
    /// </summary>
    /// <param name="other">The Collider of the GameObject being collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    /// <summary>
    /// Collider function: Called when another Collider leaves this GameObject's trigger Collider. If the other object is tagged as Interactable, remove it from the list of objects that can be interacted with currently
    /// </summary>
    /// <param name="other">The Collider of the GameObject being collided with</param>
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    /// <summary>
    /// Pick up the closest interactable object, if one exists
    /// </summary>
    public void Pickup() {

        m_CurrentInteractable = GetNearestInteractable();

        if (!m_CurrentInteractable)
            return;

        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        m_CurrentInteractable.transform.position = transform.position;

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        m_CurrentInteractable.m_ActiveHand = this;

    }

    /// <summary>
    /// Drop the currently held object, if there is one
    /// </summary>
    public void Drop() {

        if (!m_CurrentInteractable)
            return;

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

        m_Joint.connectedBody = null;


        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;

    }

    /// <summary>
    /// Returns the Interactable Componenr from the list of currently interactable objects whose position is closest to the hand
    /// </summary>
    /// <returns>The Interactable Component of the GameObject</returns>
    private Interactable GetNearestInteractable() {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach (Interactable interactable in m_ContactInteractables) {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance) {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return nearest;
    }
}
