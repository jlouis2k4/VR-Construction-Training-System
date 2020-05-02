using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Class that loads and updates the current list of saved levels on the Main Menu.
/// </summary>
public class LevelListController : MonoBehaviour
{
    // Directory containing saved levels.
    public string FileDirectory = "Saves";

    [SerializeField] private GameObject textTemplate;

    private List<GameObject> textItems;

    /// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
    private void Awake()
    {
        InitFiles();
    }

    /// <summary>
    /// Adds text entry to the list of levels.
    /// </summary>
    /// <param name="newText"></param>
    public void LogText(string newText)
    {
        GameObject newTextItem = Instantiate(textTemplate) as GameObject;
        newTextItem.SetActive(true);

        newTextItem.GetComponent<TextItem>().SetTitle(newText);
        newTextItem.transform.SetParent(transform, false);

        textItems.Add(newTextItem);
    }

    /// <summary>
    /// Retrieves non-metadata files from folder FileDirectory and adds a text entry for each to the level list.
    /// </summary>
	public void InitFiles() {
		int dirLength = Application.streamingAssetsPath.Length + FileDirectory.Length + 2;
		string curString;
		textItems = new List<GameObject>();
		var files = Directory.GetFiles(Application.streamingAssetsPath + '/' + FileDirectory);
		foreach (string file in files)
		{
			if (!file.Contains(".bin") || file.Contains(".meta")) continue;
			curString = file.Remove(0, dirLength);
			LogText(curString.Remove(curString.Length - 4));
		}
	}

    /// <summary>
    /// Rebuilds the level list from non-metadata files from folder FileDirectory.
    /// </summary>
    public void refreshFiles()
    {
        int dirLength = Application.streamingAssetsPath.Length + FileDirectory.Length + 2;
        string curString;
        foreach(GameObject gm in textItems)
        {
            GameObject.Destroy(gm);
        }
        textItems = new List<GameObject>();
        var files = Directory.GetFiles(Application.streamingAssetsPath + '/' + FileDirectory);
        foreach (string file in files)
        {
            if (file.Contains("meta")) continue;
            curString = file.Remove(0, dirLength);
            LogText(curString.Remove(curString.Length - 4));
        }
    }
}
