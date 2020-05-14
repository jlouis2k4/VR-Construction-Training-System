using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marks gameobject to be saved to .bin file by the SavableList.
/// </summary>
public class SavableObject : MonoBehaviour
{
	public string lib;
	public string objName;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		objName = gameObject.name;
	}

}
