using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.Serialization;

public enum objType
{
	nullObject = 0,
	hazard = 1,
	item = 2,
	player = 3,
	structure = 4,
}

[Serializable]
public class SavableData
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public float Rx { get; set; }
	public float Ry { get; set; }
	public float Rz { get; set; }
	public float Rw { get; set; }
	public int Type { get; set; }
	public string Name { get; set; }

	public SavableData(float ax, float ay, float az, float arx, float ary, float arz, float arw, int atype, string aname)
	{
		X = ax;
		Y = ay;
		Z = az;
		Rx = arx;
		Ry = ary;
		Rz = arz;
		Rw = arw;
		Type = atype;
		Name = aname;
	}

	public override string ToString()
	{
		return ("x: " + X + " y: " + Y + " z: " + Z + " type: " + Type);
	}
}

public class SavableList : MonoBehaviour
{
	public string levelName = "test"; // could also be made to work with the path to the file
	public List<SavableData> myObjectList;

	void Start()
	{
		myObjectList = new List<SavableData>();

	}

	// Update is called once per frame 
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			try
			{
				using (Stream stream = File.Open(Application.dataPath + "/Saves/" + levelName + ".bin", FileMode.Create))
				{
					var binForm = new BinaryFormatter();
					CompileSaveData();
					binForm.Serialize(stream, myObjectList);
				}
				Debug.Log("Saved!");
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}

		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			try
			{
				using (Stream stream = File.Open(Application.dataPath + "/saves/" + levelName + ".bin", FileMode.Open))
				{
					var binForm = new BinaryFormatter();

					myObjectList = (List<SavableData>)binForm.Deserialize(stream);
					foreach (SavableData obj in myObjectList) SpawnMyObject(obj);
				}
				Debug.Log("Loaded!");
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}

		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			foreach (Transform child in transform)
			{
				SavableObject curData = child.gameObject.GetComponent("MyObject") as SavableObject;
				if (curData != null)
				{
					Destroy(child.gameObject);
				}
			}
			Debug.Log("Destroyed!");
		}
	}

	void SpawnMyObject(SavableData data)
	{
		string path;
		Vector3 pos = new Vector3(data.X, data.Y, data.Z);
		Quaternion rot = new Quaternion(data.Rx, data.Ry, data.Rz, data.Rw);
		switch ((objType)data.Type)
		{
			case objType.hazard:
				path = Constants.HAZARD_PATH + data.Name;
				break;
			case objType.item:
				path = Constants.ITEM_PATH + data.Name;
				break;
			default:
				path = Constants.PLAYER_PATH + data.Name;
				break;
		}
		Instantiate(Resources.Load(path), pos, rot, transform);
	}

	void CompileSaveData()
	{
		SavableObject curData = new SavableObject();
		myObjectList.Clear();
		foreach (Transform child in transform)
		{
			curData = child.gameObject.GetComponent("MyObject") as SavableObject;
			if (curData != null)
			{
				myObjectList.Add(new SavableData(
					child.position.x, child.position.y, child.position.z,
					child.rotation.x, child.rotation.y, child.rotation.z, child.rotation.w,
					(int)(curData.type), curData.objName));


			}
		}
	}
}