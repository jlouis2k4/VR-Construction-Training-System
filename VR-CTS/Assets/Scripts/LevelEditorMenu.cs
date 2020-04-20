using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// Class containing the main behavior of the EditorMenu prefab.
/// </summary>
public class LevelEditorMenu : MonoBehaviour
{
    public InputField nameInputField;
	public SavableList levelManagerList;
	public GameObject menuFields;
	public GameObject overwriteMenu;

	const int MENU_SCENE = 0;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
	{
		//Sets initial text in Level Name field
		if (GlobalData.LevelName != null) nameInputField.text = GlobalData.LevelName;

		//Creates listener that raises a flag if focus is taken away from the nameInputField InputField.
		nameInputField.onEndEdit.AddListener(OnNameInputEnd);
	}

	/// <summary>
    /// Listener function that logs the string that was put into the Level Name field of the Level Editor Menu.
    /// </summary>
    /// <param name="arg0"></param>
	private void OnNameInputEnd(string arg0) {
		Debug.Log(arg0);
	}

	/// <summary>
    /// OnClick() function that handles saving the current level in the Level Editor.
    /// If trying to overwrite an existing level, enables the confirmation menu.
    /// </summary>
	public void OnSaveButtonClicked() {
		Debug.Log("Attempting to save level: " + nameInputField.text);

		bool success = levelManagerList.TrySaveLevel(nameInputField.text);
		if (!success) {
			OverwriteMenu overMenu = overwriteMenu.GetComponent<OverwriteMenu>();
			if (overMenu != null) {
				overMenu.UpdateText(nameInputField.text);
				overwriteMenu.SetActive(true);

				// prevents the user from editing editor menu fields while confirmation menu is active.
				menuFields.SetActive(false);
			}
		}
	}

	/// <summary>
    /// OnClick() function. Returns to the MainMenu Scene.
    /// </summary>
	public void onExitButtonClicked() {
		if (Application.isEditor)
		{
			EditorSceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Single);
		}
		else
		{
			SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Single);
		}
	}

	/// <summary>
    /// OnClick() function. Saves current level over existing one with the same name.
    /// </summary>
	public void onYesButtonClicked() {
		Debug.Log("Overwriting level: " + nameInputField.text);
		levelManagerList.SaveCurrentLevel(nameInputField.text);
		overwriteMenu.SetActive(false);
		menuFields.SetActive(true);
	}

	/// <summary>
    /// OnClick() function. Cancels saving the current level.
    /// </summary>
	public void onNoButtonClicked() {
		Debug.Log("Save canceled");
		overwriteMenu.SetActive(false);
		menuFields.SetActive(true);
	}
}
