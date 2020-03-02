using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	public ObjectivesListUI objectiveList;

	private int totalHazardCount;
	private int totalHazardCompleted;

	private void Awake()
	{
		totalHazardCount = 0;
		totalHazardCompleted = 0;
	}

	public void PopulateObjectives(List<Hazard> hazards) {
        Debug.Log("Populating Hazard Counter with " + hazards.Count + " hazards");
		int[] hazardCountArray = new int[Enum.GetValues(typeof(HazType)).Length];
		for (int i = 0; i < hazardCountArray.Length; i++) {
			hazardCountArray[i] = 0;
		}

		foreach (Hazard hazard in hazards) {
			hazard.HazardEvent.AddListener(HazardCompleted);
			hazardCountArray[(int)hazard.GetHazType()]++;
			totalHazardCount++;
		}
		objectiveList.RecordHazards(hazardCountArray);
	}

	void HazardCompleted(bool completed, HazType hazType)
	{
		if (completed)
		{
			totalHazardCompleted++;
		}
		else
		{
			totalHazardCompleted--;
		}
		objectiveList.UpdateCompletedCount(completed, (int)hazType);
	}

}
