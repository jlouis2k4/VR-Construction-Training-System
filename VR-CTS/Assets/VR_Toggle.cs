using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VR_Toggle : MonoBehaviour { 

    public bool VREnabled;
    public GameObject VRController;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Awake()
    {
        if (VREnabled)
        {
            EnableVR();
        } else
        {
            DisableVR();
        }
    }

    void EnableVR()
    {
        StartCoroutine(LoadDevice("OpenVR", true));
    }

    void DisableVR()
    {
        StartCoroutine(LoadDevice("", false));
    }

    IEnumerator LoadDevice(string device, bool enabled)
    {
        XRSettings.LoadDeviceByName(device);
        yield return new WaitForSeconds(0.5f);
        XRSettings.enabled = enabled;
        if (VRController)
        {
            VRController.SetActive(enabled);
            pauseMenu.SetActive(true);
        }
    }
   

}
