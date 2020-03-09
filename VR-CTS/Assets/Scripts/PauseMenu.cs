using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Valve.VR;

public class PauseMenu : MonoBehaviour
{
    public static bool LevelIsPaused = false;

	public GameObject PauseMenuUI;
    public Transform HeadsetCamera;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_PauseAction;

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
        Pointer.MenuIsActive(false);
        Time.timeScale = 1f;
		LevelIsPaused = false;
	}

	public void Pause() {
        Vector3 headsetRot = HeadsetCamera.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(headsetRot.x, headsetRot.y, 0);
        transform.position = HeadsetCamera.position + HeadsetCamera.TransformDirection(0, 0, 2);
		PauseMenuUI.SetActive(true);
        Pointer.MenuIsActive(true);
		Time.timeScale = 0f;
		LevelIsPaused = true;
	}

	public void QuitLevel() {
		GlobalData.LevelName = null;
		GlobalData.PlayerCanMove = true;
		if (Application.isEditor)
		{
			EditorSceneManager.LoadScene(0, LoadSceneMode.Single);
		}
		else
		{
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}
}
