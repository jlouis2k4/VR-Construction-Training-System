using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

/// <summary>
/// Contains the main behavior of the MainMenu prefab.
/// </summary>
public class MainMenu : MonoBehaviour
{
    //TODO: Needs to be refactored later on to allow for changes in build settings
    public const int PLAY_SCENE = 1;
    public const int EDITOR_SCENE = 2;

    public GameObject mainMenu;
    public GameObject levelMenu;
	public LevelListController levelList;

    private bool mainMenuEnabled = true;
    private bool levelMenuEnabled = false;

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    private void Awake()
    {
        mainMenu.SetActive(mainMenuEnabled);
        levelMenu.SetActive(levelMenuEnabled);
    }

    /// <summary>
    /// OnClick() function that loads the selected level into the PlayLevel scene.
    /// Does nothing if no level is selected.
    /// </summary>
    public void PlayLevel()
    {
        if (GlobalData.LevelName != null)
        {
            if (Application.isEditor)
            {
                EditorSceneManager.LoadScene(PLAY_SCENE, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(PLAY_SCENE, LoadSceneMode.Single);
            }
        }
    }

    /// <summary>
    /// OnClick() function that loads the selected level into the LevelEditor scene.
    /// Does nothing if no level is selected.
    /// </summary>
    public void EditLevel()
    {
        if (GlobalData.LevelName != null)
        {
			if (Application.isEditor)
            {
                EditorSceneManager.LoadScene(EDITOR_SCENE, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(EDITOR_SCENE, LoadSceneMode.Single);
            }
        }
    }

    /// <summary>
    /// OnClick() function that loads the LevelEditor scene without loading a level that already exists.
    /// </summary>
    public void CreateLevel()
    {
        GlobalData.LevelName = null;
        if (Application.isEditor)
        {
            EditorSceneManager.LoadScene(EDITOR_SCENE, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(EDITOR_SCENE, LoadSceneMode.Single);
        }
        
    }

    /// <summary>
    /// OnClick() function that deletes the selected level and calls for a refresh of the level list.
    /// Does nothing if no level is selected.
    /// </summary>
    public void DeleteLevel()
    {
        if (GlobalData.LevelName != null) {
			try {
				File.Delete(Application.dataPath + "/Saves/" + GlobalData.LevelName + ".bin");
				levelList.refreshFiles();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());	
			}
		}
    }

    /// <summary>
    /// OnClick() function that switches between the mainMenu and levelMenu submenus.
    /// </summary>
    public void SwapMenus()
    {
        ToggleLevelMenu();
        ToggleMainMenu();
    }

    /// <summary>
    /// Switches the enabled status of the levelMenu submenu.
    /// </summary>
    private void ToggleLevelMenu()
    {
        levelMenuEnabled = !levelMenuEnabled;
        levelMenu.SetActive(levelMenuEnabled);
    }

    /// <summary>
    /// Switches the enabled status of the mainMenu submenu.
    /// </summary>
    private void ToggleMainMenu()
    {
        mainMenuEnabled = !mainMenuEnabled;
        mainMenu.SetActive(mainMenuEnabled);
    }
}
