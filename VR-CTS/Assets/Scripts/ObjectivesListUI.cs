using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesListUI : MonoBehaviour
{
	private Text[] hazardCountTexts;
	public Transform anchor;
	public GameObject hazardText;
	public int textOffset;

	private int[] hazardCompleted;
	private int[] hazardTotals;

	public void Awake()
	{
		hazardCountTexts = new Text[Enum.GetValues(typeof(HazType)).Length];
		hazardCompleted = new int[Enum.GetValues(typeof(HazType)).Length];
		for (int i = 0; i < hazardCompleted.Length; i++) {
			hazardCompleted[i] = 0;
		}
	}


	public void RecordHazards(int[] hazardCountArray) {
		GameObject curObject;
		int currentOffset = 0;
		hazardTotals = hazardCountArray;
		for (int i = 0; i <= hazardCountArray.Length; i++) {
			if (hazardCountArray[i] > 0) {
				curObject = Instantiate(hazardText, new Vector3(anchor.position.x, anchor.position.y - currentOffset, anchor.position.z), anchor.rotation) as GameObject;
				curObject.transform.SetParent(anchor);
				hazardCountTexts[i] = curObject.GetComponent<Text>();
				SetHazardText(i);
			}
		}
	}

	public void UpdateCompletedCount(bool completed, int index) {
		if (completed) hazardCompleted[index]++;
		else hazardCompleted[index]--;
		SetHazardText(index);
	}

	public void SetHazardText(int index) {
		hazardCountTexts[index].text = Enum.GetName(typeof(HazType), index) + " Hazards Resolved: " + hazardTotals[index] + "/" + hazardTotals[index];
	}
}
