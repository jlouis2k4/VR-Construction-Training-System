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
	public float textOffset;

	private int[] hazardCompleted;
	public int[] hazardTotals;

	public void RecordHazards(int[] hazardCountArray) {
        GameObject curObject;
        float curOffset = 0;
        int count = hazardCountArray.Length;
        hazardCountTexts = new Text[count];
        hazardCompleted = new int[count];
        hazardTotals = new int[count];
        for (int i = 0; i < hazardCountArray.Length; i++) {
            hazardCompleted[i] = 0;
            hazardTotals[i] = hazardCountArray[i];
            if (hazardTotals[i] > 0) {
                curObject = (GameObject)Instantiate(hazardText, anchor.transform);
                curObject.transform.Translate(new Vector3(0, -1*curOffset, 0));
                curOffset += textOffset;
                hazardCountTexts[i] = (Text)curObject.GetComponent<Text>();
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
		hazardCountTexts[index].text = Enum.GetName(typeof(HazType), index) + " Hazards Resolved: " + hazardCompleted[index] + "/" + hazardTotals[index];
	}
}
