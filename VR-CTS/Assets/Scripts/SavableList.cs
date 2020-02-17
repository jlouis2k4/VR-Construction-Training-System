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
	public string Lib { get; set; }
	public string Name { get; set; }

	public SavableData(float ax, float ay, float az, float arx, float ary, float arz, float arw, string alib, string aname)
	{
		X = ax;
		Y = ay;
		Z = az;
		Rx = arx;
		Ry = ary;
		Rz = arz;
		Rw = arw;
		Lib = alib;
		Name = aname;
	}

	public override string ToString()
	{
		return ("x: " + X + " y: " + Y + " z: " + Z + " path: /" + Lib + "/" + Name);
	}
}

public class SavableList : MonoBehaviour
{
	public string levelName; // could also be made to work with the path to the file
	public List<SavableData> myObjectList;

    private void Awake()
    {
        levelName = GlobalData.LevelName;
    }

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
            myObjectList.Clear();
            try
			{
				using (Stream stream = File.Open(Application.dataPath + "/Saves/" + levelName + ".bin", FileMode.Open))
				{
					var binForm = new BinaryFormatter();

					myObjectList = (List<SavableData>)binForm.Deserialize(stream);
                    foreach (SavableData obj in myObjectList)
                    {
                        Debug.Log(obj.ToString());
                        SpawnMyObject(obj);
                    }
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
			SavableObject[] savables = GameObject.FindObjectsOfType<SavableObject>();
			if (savables != null)
			{
				foreach(SavableObject sav in savables) Destroy(sav.gameObject);
			}
			Debug.Log("Destroyed!");
		}
	}

	void SpawnMyObject(SavableData data)
	{
		Vector3 pos = new Vector3(data.X, data.Y, data.Z);
		Quaternion rot = new Quaternion(data.Rx, data.Ry, data.Rz, data.Rw);
		Instantiate(Resources.Load(data.Lib + "/" + data.Name), pos, rot, transform);
	}

	void CompileSaveData()
	{
		myObjectList.Clear();
		
		SavableObject[] savables = GameObject.FindObjectsOfType<SavableObject>();
		if (savables != null)
		{
            foreach (SavableObject sav in savables)
            {
                myObjectList.Add(new SavableData(
                    sav.transform.position.x, sav.transform.position.y, sav.transform.position.z,
                    sav.transform.rotation.x, sav.transform.rotation.y, sav.transform.rotation.z, sav.transform.rotation.w,
                    sav.lib, sav.objName.Remove(sav.objName.Length - 7)));
                Debug.Log(" path: /" + sav.lib + "/" + sav.objName.Remove(sav.objName.Length - 7));
            }


		}
		
	}
}