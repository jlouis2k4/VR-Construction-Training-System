using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PauseMenu : MonoBehaviour
{
    public static bool LevelIsPaused = false;

	public GameObject PauseMenuUI;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_PauseAction;

    // Update is called once per frame
    void Update()
    {
        if (m_PauseAction.GetStateDown(m_TargetSource))
        {
            if (LevelIsPaused == true) Resume();
            else Pause();
        }
    }

	public void Resume() {
		PauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		LevelIsPaused = false;
	}

	public void Pause() {
		PauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		LevelIsPaused = true;
	}

	public void QuitLevel() {
		Debug.Log("Quitting App...");
		Application.Quit();
	}
}
