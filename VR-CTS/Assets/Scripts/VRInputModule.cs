using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

/// <summary>
/// Subclass of Unity's BaseInputModule that handles user input events in VR.
/// </summary>
public class VRInputModule : BaseInputModule {
    
	public Camera m_Camera;
	public SteamVR_Input_Sources m_TargetSource;
	public SteamVR_Action_Boolean m_ClickAction;

	private GameObject m_CurrentObject = null;
	private PointerEventData m_Data = null;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	protected override void Awake() {
		base.Awake();

		m_Data = new PointerEventData(eventSystem);
	}

	/// <summary>
    /// Processes user input.
    /// Treats the m_ClickAction action from the VR Controllers as a click from the mouse 
    /// located at a raycast pointing where the user is aiming the controller with the Pointer attached.
    /// </summary>
	public override void Process() {
		
		// Reset Data, set Camera
		m_Data.Reset();
		m_Data.position = new Vector2(m_Camera.pixelWidth / 2, m_Camera.pixelHeight / 2);
		
		// Raycast
		eventSystem.RaycastAll(m_Data, m_RaycastResultCache);
		m_Data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		m_CurrentObject = m_Data.pointerCurrentRaycast.gameObject;

		// Clear
		m_RaycastResultCache.Clear();

		// Hover
		HandlePointerExitAndEnter(m_Data, m_CurrentObject);

		// Press
		if (m_ClickAction.GetStateDown(m_TargetSource)) ProcessPress(m_Data);

		// Release
		if (m_ClickAction.GetStateUp(m_TargetSource)) ProcessRelease(m_Data);
	}

	/// <summary>
    /// Returns current information about where the user is pointing.
    /// </summary>
    /// <returns></returns>
	public PointerEventData GetData() {
		return m_Data;
	}

	/// <summary>
	/// Processes when the user presses down with the button attached to the m_clickAction action on the VR Controllers.
	/// </summary>
	/// <param name="data">current information about where the user is pointing.</param>
	private void ProcessPress(PointerEventData data) {

		// Set Raycast
		data.pointerPressRaycast = data.pointerCurrentRaycast;

		// Check for hit, get down handler, call
		GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(m_CurrentObject, data, ExecuteEvents.pointerDownHandler);

		// If no down handler, get click handler
		if (newPointerPress == null) {
			newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);
		}

		// Set data
		data.pressPosition = data.position;
		data.pointerPress = newPointerPress;
		data.rawPointerPress = m_CurrentObject;
	}

	/// <summary>
	/// Processes when the user presses releases the button attached to the m_clickAction action on the VR Controllers.
	/// </summary>
	/// <param name="data">current information about where the user is pointing.</param>
	private void ProcessRelease(PointerEventData data) {

		// Execute pointer up
		ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

		// Check for click handler
		GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(m_CurrentObject);

		// Check if actual
		if (data.pointerPress == pointerUpHandler) {
			ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
		}

		// Clear selected gameobject
		eventSystem.SetSelectedGameObject(null);

		// Reset
		data.pressPosition = Vector2.zero;
		data.pointerPress = null;
		data.rawPointerPress = null;


	}
}
