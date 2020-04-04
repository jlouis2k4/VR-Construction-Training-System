using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public Image m_HardHat;
	public Image m_Goggles;
	public Image m_Vest;

    public void Start()
    {
        m_HardHat.enabled = false;
        m_Vest.enabled = false;
        m_Goggles.enabled = false;
    }

    public void ToggleHardHat(bool isActive) {
		m_HardHat.enabled = isActive;
	}

	public void ToggleGoggles(bool isActive) {
		m_Goggles.enabled = isActive;
	}

	public void ToggleVest(bool isActive) {
		m_Vest.enabled = isActive;
	}
}
