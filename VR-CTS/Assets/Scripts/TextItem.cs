using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class representing a text entry in the list of levels in the MainMenu scene.
/// </summary>
public class TextItem : MonoBehaviour
{
    public Text title;
    public Text date; //TODO: Figure out how to get the 'date modified' metadata from a file.

    /// <summary>
    /// Sets the name of the level field.
    /// </summary>
    /// <param name="textTitle">The name of the level.</param>
    public void SetTitle(string textTitle)
    {
        title.text = textTitle;
    }

    /// <summary>
    /// OnClick() function that sets the name of the currently selected level to title.
    /// </summary>
    public void Selectlevel()
    {
        GlobalData.LevelName = title.text;
    }
}
