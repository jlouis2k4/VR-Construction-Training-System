using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
	public float m_DefaultLength = 5.0f;
	public GameObject m_Dot;
	public VRInputModule m_InputModule;

	private LineRenderer m_LineRenderer;

	private void Awake()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
    {
		if (PauseMenu.LevelIsPaused) {
            m_LineRenderer.enabled = true;
            if (m_Dot.activeInHierarchy == false) m_Dot.SetActive(true);
            UpdateLine();
		} else {
            m_LineRenderer.enabled = false;
			if (m_Dot.activeInHierarchy == true) m_Dot.SetActive(false);
		}
    }

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

	private RaycastHit CreateRaycast(float length) {
		RaycastHit hit;
		Ray ray = new Ray(transform.position, transform.forward);
		Physics.Raycast(ray, out hit, m_DefaultLength);
		return hit;
	}
}
