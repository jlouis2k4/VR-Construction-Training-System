using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class containing data persistant between scenes
/// </summary>
public static class GlobalData
{
    private static string _levelName = null;

	/// <summary>
    /// Contains the name of the currently loaded level.
    /// </summary>
    public static string LevelName {
		get { return _levelName; }
		set {
			Debug.Log("Level: " + value);
			_levelName = value;
		}
	}

	/// <summary>
    /// Whether the player can move from their current position.
    /// </summary>
	public static bool PlayerCanMove = true;

}
