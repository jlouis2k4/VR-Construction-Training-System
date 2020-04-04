using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelListController : MonoBehaviour
{
    public string FileDirectory = "Saves";

    [SerializeField] private GameObject textTemplate;

    private List<GameObject> textItems;

    private void Awake()
    {
        InitFiles();
    }

    public void LogText(string newText)
    {
        GameObject newTextItem = Instantiate(textTemplate) as GameObject;
        newTextItem.SetActive(true);

        newTextItem.GetComponent<TextItem>().SetTitle(newText);
        newTextItem.transform.SetParent(transform, false);

        textItems.Add(newTextItem);
    }

	public void InitFiles() {
		int dirLength = Application.dataPath.Length + FileDirectory.Length + 2;
		string curString;
		textItems = new List<GameObject>();
		var files = Directory.GetFiles(Application.dataPath + '/' + FileDirectory);
		foreach (string file in files)
		{
			if (file.Contains("meta")) continue;
			curString = file.Remove(0, dirLength);
			LogText(curString.Remove(curString.Length - 4));
		}
	}

    public void refreshFiles()
    {
        int dirLength = Application.dataPath.Length + FileDirectory.Length + 2;
        string curString;
        foreach(GameObject gm in textItems)
        {
            GameObject.Destroy(gm);
        }
        textItems = new List<GameObject>();
        var files = Directory.GetFiles(Application.dataPath + '/' + FileDirectory);
        foreach (string file in files)
        {
            if (file.Contains("meta")) continue;
            curString = file.Remove(0, dirLength);
            LogText(curString.Remove(curString.Length - 4));
        }
    }
}
