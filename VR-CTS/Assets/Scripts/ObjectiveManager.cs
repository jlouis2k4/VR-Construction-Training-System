using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	private const int MAX_TIME_SECONDS = 6039; // 99:99
	private const int BASE_SCORE_MULTIPLIER = 10;
	private const float POINTS_SECONDS_RATIO = 1 / 60; // number of points per number of seconds

	public ObjectivesListUI objectiveList;
	public GameTimer gameTimer;

	private int totalHazardCount;
	private int totalHazardCompleted;
	private int deathCount;

	private void Awake()
	{
		totalHazardCount = 0;
		totalHazardCompleted = 0;
		deathCount = 0;
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
		gameTimer.StartTimer();
	}

	public void HazardCompleted(bool completed, HazType hazType)
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

	public void PlayerDied() {
		deathCount++;
	}

	public int GetScore () {
		int timeScore = Mathf.FloorToInt(POINTS_SECONDS_RATIO * (MAX_TIME_SECONDS - gameTimer.getFinalTime()));
		return BASE_SCORE_MULTIPLIER * (totalHazardCompleted - deathCount) + timeScore;
	}

}
