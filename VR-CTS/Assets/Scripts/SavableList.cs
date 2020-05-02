using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Runtime.Serialization;

/// <summary>
/// Serializable data-storage class that contains information about an object being written-to/read-from a binary file.
/// </summary>
[Serializable]
public class SavableData
{
	// Position (Vector3)
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }

	// Rotation (Quaternion)
	public float Rx { get; set; }
	public float Ry { get; set; }
	public float Rz { get; set; }
	public float Rw { get; set; }

	// Directory in Resources folder where the asset prefab is stored
	public string Lib { get; set; }

	// Name of the asset's prefab
	public string Name { get; set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="ax">x-coordinate of object.</param>
	/// <param name="ay">y-coordinate of object.</param>
	/// <param name="az">z-coordinate of object.</param>
	/// <param name="arx">x-axis rotation of object.</param>
	/// <param name="ary">y-axis rotation of object.</param>
	/// <param name="arz">z-axis rotation of object.</param>
	/// <param name="arw">scalar of rotation of object.</param>
	/// <param name="alib">directory in Resources folder where the asset prefab of the object is stored.</param>
	/// <param name="aname">name of the object's prefab</param>
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

	/// <summary>
    /// Concatenates position and file path of SavableData object and returns a string.
    /// </summary>
    /// <returns> A string containing the position and file path of the object.</returns>
	public override string ToString()
	{
		return ("x: " + X + " y: " + Y + " z: " + Z + " path: /" + Lib + "/" + Name);
	}
}

/// <summary>
/// Class that loads and saves data of objects in a level.
/// </summary>
public class SavableList : MonoBehaviour
{
	public string levelName; // could also be made to work with the path to the file
	public bool isPlayMode = false;
	public ObjectiveManager objectiveManager; // only use if in PlayMode
	private List<SavableData> myObjectList;
	private List<Hazard> myHazardList;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
    {
		myObjectList = new List<SavableData>();
        myHazardList = new List<Hazard>();

		// Get name of current level, if it exists.
		levelName = GlobalData.LevelName;

		// A current level exists and needs to be loaded.
		if (levelName != null) {

			// Try to load the level from binary file.
			bool success = TryLoadLevel();
			if (success && isPlayMode) {
                Debug.Log("Play Mode");
				// If in play mode, pass the list of hazards loaded to the ObjectiveManager.
				objectiveManager.PopulateObjectives(myHazardList);
			}
		}
    }

	/// <summary>
    /// Instantiates an instance of a prefab based on a SavableData object loaded from a binary file.
    /// </summary>
    /// <param name="data">The SavableData containing information about the object to be instantiated.</param>
    /// <returns> The instantiated gameobject. </returns>
	GameObject SpawnMyObject(SavableData data)
	{
		// Instantiate gameobject
		Vector3 pos = new Vector3(data.X, data.Y, data.Z);
		Quaternion rot = new Quaternion(data.Rx, data.Ry, data.Rz, data.Rw);
		GameObject obj = Instantiate(Resources.Load(data.Lib + "/" + data.Name), pos, rot, transform) as GameObject;

		// Add a SavableObject component to the gameobject if it does not have one so that it can be loaded/saved later on.
		SavableObject savable = obj.GetComponent<SavableObject>();
		if (savable == null)
		{
			savable = obj.AddComponent<SavableObject>();
		}
        Debug.Log("Load From Lib: " + data.Lib);
		savable.lib = data.Lib;
        savable.objName = data.Name;       
		return obj;
	}

	/// <summary>
    /// Finds all gameobjects with the SavableObject component and records their information as SavableData objects.
    /// Stores SavableData in private list.
    /// </summary>
	void CompileSaveData()
	{
		myObjectList.Clear();
		
		// Retrieves all SavableObject components found in the scene.
		SavableObject[] savables = GameObject.FindObjectsOfType<SavableObject>();
		if (savables != null)
		{
            // For each SavableObject create a new SavableData instance that contains its position, rotation, and path data.
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

	/// <summary>
	/// Tries to serialize objects into .bin file.
    /// If a file already exists with the same name, it returns false, otherwise true.
	/// </summary>
	/// <param name="newName">The name of the level to be saved.</param>
	/// <returns>Returns false if a level with the given name already exists. Otherwise returns true.</returns>
	public bool TrySaveLevel(string newName) {
		// Tries to create a new binary file. Throws exception if a file with the same name as 'newname' exists.
		try
		{
			// Write SavableData objects as binary data to file
			using (Stream stream = File.Open(Application.streamingAssetsPath + "/Saves/" + newName + ".bin", FileMode.CreateNew))
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

	/// <summary>
	/// Serializes objects into .bin file, overwriting a file with the same name
	/// </summary>
	/// <param name="curName">The name of the level to be saved.</param>
	public void SaveCurrentLevel(string curName) {
		try
		{
			// Write SavableData objects as binary data to file.
			using (Stream stream = File.Open(Application.streamingAssetsPath + "/Saves/" + curName + ".bin", FileMode.Create))
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

	/// <summary>
	/// Tries to load objects from .bin file with the name stored in GlobalData.
    /// If file cannot be opened returns false, otherwise true.
	/// </summary>
	/// <returns>If file cannot be opened returns false, otherwise true.</returns>
	public bool TryLoadLevel() {
		myObjectList.Clear();
		try
		{
			// Read SavableData objects from binary file if it exists.
			using (Stream stream = File.Open(Application.streamingAssetsPath + "/Saves/" + levelName + ".bin", FileMode.Open))
			{
				var binForm = new BinaryFormatter();

				myObjectList = (List<SavableData>)binForm.Deserialize(stream);
				foreach (SavableData obj in myObjectList)
				{
					Debug.Log(obj.ToString());
					SpawnMyObject(obj);
				}

                // If in play mode, get all the hazards.
                if (isPlayMode)
                {
                    myHazardList = new List<Hazard>(GameObject.FindObjectsOfType<Hazard>());
                    foreach(Hazard h in myHazardList)
                    {
                        print("Found Hazard: " + h.gameObject.name);
                    }
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