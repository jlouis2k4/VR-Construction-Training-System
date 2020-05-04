using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VR_Toggle : MonoBehaviour { 

    public bool VREnabled;
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
        yield return null;
        XRSettings.enabled = enabled;
    }
   

}
