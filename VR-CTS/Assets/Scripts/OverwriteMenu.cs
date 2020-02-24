using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverwriteMenu : MonoBehaviour
{
    public Text text;

	public void UpdateText(string levelName) {
		text.text = "A file named \n" + levelName + "\n already exists. Do you want to overwrite that file ?";
	}
}
