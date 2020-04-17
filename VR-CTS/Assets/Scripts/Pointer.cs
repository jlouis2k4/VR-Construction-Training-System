using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class containing the behavior of the line pointer that extends
/// from the VR Controller to help the user interact with VR menus.
/// </summary>
public class Pointer : MonoBehaviour
{
    // Static counter of how many menus are currently active in the scene.
	private static int activeMenuCounter = 0;

    public float m_DefaultLength = 5.0f;
	public GameObject m_Dot;
	public VRInputModule m_InputModule;

	private LineRenderer m_LineRenderer;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	/// <summary>
	/// Update is called once every frame.
	/// </summary>
	private void Update()
    {
		// If at least one menu is active in the level, enable the pointer.
		if (activeMenuCounter > 0) {
            m_LineRenderer.enabled = true;
            if (m_Dot.activeInHierarchy == false) m_Dot.SetActive(true);
            UpdateLine();
		} else {
            m_LineRenderer.enabled = false;
			if (m_Dot.activeInHierarchy == true) m_Dot.SetActive(false);
		}
    }

	/// <summary>
    /// Update the line created via raycast pointing out from the VR Controller.
    /// </summary>
	private void UpdateLine() {
		// Use default or distance
		PointerEventData data = m_InputModule.GetData();
		float targetLength = data.pointerCurrentRaycast.distance == 0 ? m_DefaultLength : data.pointerCurrentRaycast.distance;

		// Raycast
		RaycastHit hit = CreateRaycast(targetLength);

		// Default
		Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Based on Hit
        if (hit.collider != null)
        {
            endPosition = hit.point;
            Debug.Log("HIT");
        }

        // Set Dot
        m_Dot.transform.position = endPosition;

		// Set Linerenderer
		m_LineRenderer.SetPosition(0, transform.position);
		m_LineRenderer.SetPosition(1, endPosition);
	}

	/// <summary>
    /// Create raycast of specified length that extends forward from this gameobject.
    /// </summary>
    /// <param name="length"> The length of the raycast to be created. </param>
    /// <returns></returns>
	private RaycastHit CreateRaycast(float length) {
		RaycastHit hit;
		Ray ray = new Ray(transform.position, transform.forward);
		Physics.Raycast(ray, out hit, m_DefaultLength);
		return hit;
	}

	/// <summary>
    /// Static function that increments/decrements the number of active menus in the level.
    /// Called from the menu scripts.
    /// </summary>
    /// <param name="isActive"> Whether the menu is active. </param>
    public static void MenuIsActive(bool isActive) {
        if (isActive) {
            activeMenuCounter++;
        }
        else activeMenuCounter--;

		if (activeMenuCounter > 0) {
			GlobalData.PlayerCanMove = false;
		} else {
			GlobalData.PlayerCanMove = true;
		}
    }
}
