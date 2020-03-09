using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class ExitMenu : MonoBehaviour
{
	public GameObject endLevelMenu;
	public GameObject scoreMenu;
	public Text scoreText;
	public ObjectiveManager objManager;

	private void Awake() {
		endLevelMenu.SetActive(false);
		scoreMenu.SetActive(false);
	}

	public void EnableConfirmationMenu(Vector3 lookAt) {
		if (endLevelMenu.activeInHierarchy == false) {
			endLevelMenu.SetActive(true);
			transform.LookAt(lookAt, Vector3.up);
			Pointer.MenuIsActive(true);
		}
	}

	public void YesEndLevel() {
		endLevelMenu.SetActive(false);
		scoreMenu.SetActive(true);
	}

	public void NoEndLevel() {
		scoreText.text = "Score: " + objManager.GetScore().ToString();
		endLevelMenu.SetActive(false);
		Pointer.MenuIsActive(false);
	}

	public void ExitLevel() {
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
