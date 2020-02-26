using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelEditorMenu : MonoBehaviour
{
    public InputField nameInputField;
	public SavableList levelManagerList;
	public GameObject menuFields;
	public GameObject overwriteMenu;

	const int MENU_SCENE = 0;

	private void Awake()
	{
		if (GlobalData.LevelName != null) nameInputField.text = GlobalData.LevelName;
		nameInputField.onEndEdit.AddListener(OnNameInputEnd);
	}

	private void OnNameInputEnd(string arg0) {
		Debug.Log(arg0);
	}

	public void OnSaveButtonClicked() {
		Debug.Log("Attempting to save level: " + nameInputField.text);
		bool success = levelManagerList.TrySaveLevel(nameInputField.text);
		if (!success) {
			OverwriteMenu overMenu = overwriteMenu.GetComponent<OverwriteMenu>();
			if (overMenu != null) {
				overMenu.UpdateText(nameInputField.text);
				overwriteMenu.SetActive(true);
				menuFields.SetActive(false);
			}
		}
	}

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

	public void onYesButtonClicked() {
		Debug.Log("Overwriting level: " + nameInputField.text);
		levelManagerList.SaveCurrentLevel(nameInputField.text);
		overwriteMenu.SetActive(false);
		menuFields.SetActive(true);
	}

	public void onNoButtonClicked() {
		Debug.Log("Save canceled");
		overwriteMenu.SetActive(false);
		menuFields.SetActive(true);
	}
}
