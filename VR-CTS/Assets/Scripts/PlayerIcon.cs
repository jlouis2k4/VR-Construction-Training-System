using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public Image m_HardHat;
	public Image m_Goggles;
	public Image m_Vest;

    public void ToggleHardHat(bool isActive) {
		m_HardHat.enabled = isActive;
	}

	public void ToggleGoggles(bool isActive) {
		m_HardHat.enabled = isActive;
	}

	public void ToggleVest(bool isActive) {
		m_HardHat.enabled = isActive;
	}
}
