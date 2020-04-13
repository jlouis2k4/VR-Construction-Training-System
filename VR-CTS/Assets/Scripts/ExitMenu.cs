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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnableConfirmationMenu();
        }
    }

    public void EnableConfirmationMenu() {
		if (endLevelMenu.activeInHierarchy == false) {
			endLevelMenu.SetActive(true);
			//transform.LookAt(new Vector3(0,1,-3), Vector3.up);
			Pointer.MenuIsActive(true);
		}
	}

	public void YesEndLevel() {
		endLevelMenu.SetActive(false);
		scoreText.text = "Score: " + objManager.GetScore().ToString();
		scoreMenu.SetActive(true);
		
	}

	public void NoEndLevel() {
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
