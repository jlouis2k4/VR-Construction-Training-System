using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool LevelIsPaused = false;

	public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        
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
