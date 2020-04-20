using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Script for enabling/disabling VR between scenes
/// </summary>
public class VRToggle : MonoBehaviour
{
    public bool VREnabled;

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    public void Awake()
    {
        if (VREnabled)
        {
            EnableVR();
        } else
        {
            DisableVR();
        }
    }

    /// <summary>
    /// Sets the XR device to be used.
    /// </summary>
    /// <param name="newDevice">The name of the XR device to be used. If an empty string is passed, used no XR device.</param>
    /// <param name="enable">Enable XRSettings.</param>
    /// <returns></returns>
    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = enable;
    }

    /// <summary>
    /// Enables SteamVR.
    /// </summary>
    void EnableVR()
    {
        StartCoroutine(LoadDevice("OpenVR", true));
    }

    /// <summary>
    /// Disables SteamVR.
    /// </summary>
    void DisableVR()
    {
        StartCoroutine(LoadDevice("", false));
    }
}
