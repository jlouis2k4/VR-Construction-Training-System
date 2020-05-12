using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class representing the player silhouette icon on the pause menu.
/// </summary>
public class PlayerIcon : MonoBehaviour
{
    public Image m_HardHat;
	public Image m_Goggles;
	public Image m_Vest;

	/// <summary>
    /// Start is called on the frame when a script is enabled.
    /// </summary>
    public void Start()
    {
        // Disable safety equipment icons by default.
		m_HardHat.enabled = false;
        m_Vest.enabled = false;
        m_Goggles.enabled = false;
    }

	/// <summary>
    /// Enables the Hard Hat icon if currently diabled, and vice-versa.
    /// </summary>
    /// <param name="isActive">The icon is active</param>
    public void ToggleHardHat(bool isActive) {
		m_HardHat.enabled = isActive;
	}

    /// <summary>
    /// Enables the Safety Goggles icon if currently diabled, and vice-versa.
    /// </summary>
    /// <param name="isActive">The icon is active</param>
	public void ToggleGoggles(bool isActive) {
		m_Goggles.enabled = isActive;
	}

    /// <summary>
    /// Enables the Safety Vest icon if currently diabled, and vice-versa.
    /// </summary>
    /// <param name="isActive">The icon is active</param>
	public void ToggleVest(bool isActive) {
		m_Vest.enabled = isActive;
	}
}
