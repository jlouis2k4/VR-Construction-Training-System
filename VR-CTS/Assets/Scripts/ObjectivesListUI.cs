using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that manages the UI on the Pause Menu that states how many of each type of hazard exists in the level,
/// and how many of each type of hazard has been completed by the user.
/// </summary>
public class ObjectivesListUI : MonoBehaviour
{
	private Text[] hazardCountTexts;
	public Transform anchor;
	public GameObject hazardText;
	public float textOffset;

	private int[] hazardCompleted;
	public int[] hazardTotals;

    /// <summary>
    /// Takes an array with an element for each value of the HazType Enum that states how many of each hazard exists in the level.
    /// For each non-zero element in the array, a text object is instantiated in the Pause Menu that tracks how many of that hazard has been completed.
    /// </summary>
    /// <param name="hazardCountArray"> The array that contains the number of each type of hazard in the level. </param>
	public void RecordHazards(int[] hazardCountArray) {
        GameObject curObject;
        float curOffset = 0;
        int count = hazardCountArray.Length;

        // Create arrays for tracking and displaying the number of hazards completed.
        hazardCountTexts = new Text[count];
        hazardCompleted = new int[count];
        hazardTotals = new int[count];

        // Iterates through array
        for (int i = 0; i < hazardCountArray.Length; i++) {
            hazardCompleted[i] = 0;
            hazardTotals[i] = hazardCountArray[i];

            // Only create new text entry if at least one hazard of the current type exists
            if (hazardTotals[i] > 0) {

                // Instantiate text entry Gameobject from prefab and set its Text component
                curObject = (GameObject)Instantiate(hazardText, anchor.transform);
                curObject.transform.Translate(new Vector3(0, -1*curOffset, 0));
                curOffset += textOffset;
                hazardCountTexts[i] = (Text)curObject.GetComponent<Text>();
                SetHazardText(i);
            }
        }
	}

    /// <summary>
    /// Updates the number of hazards completed for the specified type of hazard.
    /// </summary>
    /// <param name="completed"> The new completion status of the hazard. </param>
    /// <param name="hazType"> The type of the hazard. </param>
	public void UpdateCompletedCount(bool completed, int hazType) {
		if (completed) hazardCompleted[hazType]++;
		else hazardCompleted[hazType]--;
		SetHazardText(hazType);
	}

    /// <summary>
    /// Updates the Text of the text entry with the specified index based on recorded values.
    /// </summary>
    /// <param name="index"> The index of the text entry. </param>
	private void SetHazardText(int index) {
		hazardCountTexts[index].text = Enum.GetName(typeof(HazType), index) + " Hazards Resolved: " + hazardCompleted[index] + "/" + hazardTotals[index];
	}
}
