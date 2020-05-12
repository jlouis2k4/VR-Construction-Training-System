using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Class that handles switching between VR and Non-VR
/// </summary>
public class VR_Toggle : MonoBehaviour 
{ 

    public bool VREnabled;

    // Problematic GameObjects that need to be loaded after VR is set up. Room for improvement
    public GameObject VRController;
    public GameObject pauseMenu;

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    void Awake() 
    {
        if (VREnabled) EnableVR();
        else DisableVR();
    }

    /// <summary>
    /// Starts LoadDevice coroutine with OpenVR to enable Virtual Reality
    /// </summary>
    void EnableVR() 
    {
        StartCoroutine(LoadDevice("OpenVR", true));
    }

    /// <summary>
    /// Starts LoadDevice coroutine with no device to disable Virtual Reality
    /// </summary>
    void DisableVR() 
    {
        StartCoroutine(LoadDevice("", false));
    }

    /// <summary>
    /// Coroutine that changes the current Virtual Reality device used by the application
    /// </summary>
    /// <param name="device">String that matches the name of a VR device in the project settings</param>
    /// <param name="enabled">Enable VR</param>
    /// <returns></returns>
    IEnumerator LoadDevice(string device, bool enabled) 
    {
        XRSettings.LoadDeviceByName(device);
        yield return new WaitForSeconds(0.5f);
        XRSettings.enabled = enabled;

        if (VRController && pauseMenu) 
        {
            VRController.SetActive(enabled);
            pauseMenu.SetActive(true);
        }
    }
}
