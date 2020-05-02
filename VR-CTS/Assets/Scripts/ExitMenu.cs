using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Class containing the main behavior of the Exit Menu Prefab.
/// </summary>
public class ExitMenu : MonoBehaviour
{
	public GameObject endLevelMenu;
	public GameObject scoreMenu;
	public Text scoreText;
	public ObjectiveManager objManager;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake() {
		endLevelMenu.SetActive(false);
		scoreMenu.SetActive(false);
	}

	/// <summary>
    /// Enables the endLevelMenu attached to this GameObject and the menu pointer.
    /// </summary>
    public void EnableConfirmationMenu() {
		if (endLevelMenu.activeInHierarchy == false) {
			endLevelMenu.SetActive(true);
			Pointer.MenuIsActive(true);
		}
	}

	/// <summary>
    /// OnClick() function that disables the endLevelMenu GameObject, enables the scoreMenu GameObject,
    /// and retrieves the player's score from the ObjectiveManager.
    /// </summary>
	public void YesEndLevel() {
		endLevelMenu.SetActive(false);
		scoreText.text = "Score: " + objManager.GetScore().ToString();
		scoreMenu.SetActive(true);
		
	}

	/// <summary>
	/// OnClick() function that disables the endLevelMenu attached to this GameObject and the menu pointer.
	/// </summary>
	public void NoEndLevel() {
		endLevelMenu.SetActive(false);
		Pointer.MenuIsActive(false);
	}

	/// <summary>
    /// OnClick() function that loads the MainMenu scene.
    /// </summary>
	public void ExitLevel() {
		GlobalData.LevelName = null;
		GlobalData.PlayerCanMove = true;
#if UNITY_EDITOR

        EditorSceneManager.LoadScene(0, LoadSceneMode.Single);
#else
        SceneManager.LoadScene(0, LoadSceneMode.Single);
#endif
    }

    //TODO: Set up Collider that calls EnableConfirmationMenu when the player intersects it.
}
