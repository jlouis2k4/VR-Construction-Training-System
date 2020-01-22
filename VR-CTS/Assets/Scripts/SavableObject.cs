using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableObject : MonoBehaviour
{
	public objType type;
	public string objName;

	void Awake()
	{
		objName = gameObject.name;
	}

}
