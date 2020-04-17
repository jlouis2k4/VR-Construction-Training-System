using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing behavior for the confirmation menu in the LevelEditor scene.
/// </summary>
public class OverwriteMenu : MonoBehaviour
{
    public Text text;

	/// <summary>
    /// Updates the text in the confirmation menu that asks whether the player wants to overwrite an existing level.
    /// </summary>
    /// <param name="levelName">The name of the current level.</param>
	public void UpdateText(string levelName) {
		text.text = "A file named \n" + levelName + "\n already exists. Overwrite?";
	}
}
