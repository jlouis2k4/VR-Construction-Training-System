using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //TODO: Needs to be refactored later on to allow for changes in build settings
    public const int PLAY_SCENE = 1;
    public const int EDITOR_SCENE = 2;

    public GameObject mainMenu;
    public GameObject levelMenu;

    private bool mainMenuEnabled = true;
    private bool levelMenuEnabled = false;

    private void Awake()
    {
        mainMenu.SetActive(mainMenuEnabled);
        levelMenu.SetActive(levelMenuEnabled);
    }

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

    public void CreateLevel()
    {
        GlobalData.LevelName = null;
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

    public void DeleteLevel()
    {
        if (GlobalData.LevelName != null) Debug.Log("TODO: Delete " + GlobalData.LevelName);
    }

    public void SwapMenus()
    {
        ToggleLevelMenu();
        ToggleMainMenu();
    }

    private void ToggleLevelMenu()
    {
        levelMenuEnabled = !levelMenuEnabled;
        levelMenu.SetActive(levelMenuEnabled);
    }

    private void ToggleMainMenu()
    {
        mainMenuEnabled = !mainMenuEnabled;
        mainMenu.SetActive(mainMenuEnabled);
    }
}
