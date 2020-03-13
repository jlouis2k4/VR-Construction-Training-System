using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.Serialization;

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
	public bool isPlayMode = false;
	public ObjectiveManager objectiveManager; // only use if in PlayMode
	private List<SavableData> myObjectList;
	private List<Hazard> myHazardList;

    private void Awake()
    {
		myObjectList = new List<SavableData>();
        myHazardList = new List<Hazard>();
		levelName = GlobalData.LevelName;
		if (levelName != null) {
			bool success = TryLoadLevel();
			if (success && isPlayMode) {
                Debug.Log("Play Mode");
				objectiveManager.PopulateObjectives(myHazardList);
			}
		}
    }

	GameObject SpawnMyObject(SavableData data)
	{
		Vector3 pos = new Vector3(data.X, data.Y, data.Z);
		Quaternion rot = new Quaternion(data.Rx, data.Ry, data.Rz, data.Rw);
		GameObject obj = Instantiate(Resources.Load(data.Lib + "/" + data.Name), pos, rot, transform) as GameObject;
		SavableObject savable = obj.GetComponent<SavableObject>();
		if (savable == null)
		{
			savable = obj.AddComponent<SavableObject>();
		}
        Debug.Log("Load From Lib: " + data.Lib);
		savable.lib = data.Lib;
        savable.objName = data.Name;
		if (isPlayMode) {
			Hazard hazard = obj.GetComponent<Hazard>();
			if (hazard != null) {
				myHazardList.Add(hazard);
			}
		}
       
		return obj;
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
                    sav.lib, sav.objName));
                Debug.Log(" path: /" + sav.lib + "/" + sav.objName);
                //.Remove(sav.objName.Length - 7)
            }


        }
		
	}

	//Tries to serialize objects into .bin file. If a file already exists with the same name, it returns false, otherwise true
	public bool TrySaveLevel(string newName) {
		try
		{
			using (Stream stream = File.Open(Application.dataPath + "/Saves/" + newName + ".bin", FileMode.CreateNew))
			{
				var binForm = new BinaryFormatter();
				CompileSaveData();
				binForm.Serialize(stream, myObjectList);
			}
			Debug.Log("Saved!");
			return true;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
			return false;
		}
	}

	//Serializes objects into .bin file, overwriting a file with the same name
	public void SaveCurrentLevel(string curName) {
		try
		{
			using (Stream stream = File.Open(Application.dataPath + "/Saves/" + curName + ".bin", FileMode.Create))
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
	
	//Tries to load objects from .bin file. If file cannot be opened returns false, otherwise true
	public bool TryLoadLevel() {
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
			return true;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
			return false;
		}
	}
}