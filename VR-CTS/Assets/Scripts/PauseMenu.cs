using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using Valve.VR;

/// <summary>
/// Class containing the main behavior of the PauseMenu prefab.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool LevelIsPaused = false;

	public GameObject PauseMenuUI;
    public Transform HeadsetCamera;
    public SteamVR_Input_Sources m_TargetSource;
    public SteamVR_Action_Boolean m_PauseAction;
	public GameTimer m_GameTimer;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>s
	void Awake()
	{
			m_GameTimer.StartTimer();
	}

	/// <summary>
	/// Update is called once every frame.
	/// </summary>
	void Update()
    {
        if (m_PauseAction != null)
        {
            // If the m_PauseAction action of the VR controller is pressed down, change the state of the Pause Menu.
            if (m_PauseAction.GetStateDown(m_TargetSource))
            {
                if (LevelIsPaused == true) Resume();
                else Pause();
            }
        }
    }

	/// <summary>
	/// Disable pause menu UI and unpause the level.
	/// </summary>
	public void Resume() {
		PauseMenuUI.SetActive(false);
        Pointer.MenuIsActive(false);
        Time.timeScale = 1f;
		LevelIsPaused = false;
	}

	/// <summary>
    /// Enable pause menu UI and pause the level.
    /// Sets the position of the pause menu to in front the user, facing them.
    /// </summary>
	public void Pause() {
        Vector3 headsetRot = HeadsetCamera.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(headsetRot.x, headsetRot.y, 0);
        transform.position = HeadsetCamera.position + HeadsetCamera.TransformDirection(0, 0, 2);
		PauseMenuUI.SetActive(true);
        Pointer.MenuIsActive(true);
		Time.timeScale = 0f;
		LevelIsPaused = true;
	}

	/// <summary>
    /// Loads the MainMenu scene.
    /// </summary>
	public void QuitLevel() {
		GlobalData.LevelName = null;
		GlobalData.PlayerCanMove = true;
#if UNITY_EDITOR

        EditorSceneManager.LoadScene(0, LoadSceneMode.Single);
#else
        SceneManager.LoadScene(0, LoadSceneMode.Single);
#endif
	}
}
