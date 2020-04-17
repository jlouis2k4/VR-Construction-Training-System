using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	private const int MAX_TIME_SECONDS = 7200; // 2 hours
	private const int BASE_SCORE_MULTIPLIER = 1;
	private const float POINTS_SECONDS_RATIO = 1 / 1; // number of points per number of seconds

	public ObjectivesListUI objectiveList;
	public GameTimer gameTimer;

	private int totalHazardCount;
	private int totalHazardCompleted;
	private int deathCount;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
	{
		totalHazardCount = 0;
		totalHazardCompleted = 0;
		deathCount = 0;
	}

	/// <summary>
    /// Takes a list of Hazard objects, separates them by type, and passes them to the ObjectivesListUI.
    /// Creates listeners called when a hazard's completion status changes.
    /// </summary>
    /// <param name="hazards"> The list of Hazard objects to record and track.</param>
	public void PopulateObjectives(List<Hazard> hazards) {
        Debug.Log("Populating Hazard Counter with " + hazards.Count + " hazards");

		// Create array to record how many of each type of hazard exists.
		int[] hazardCountArray = new int[Enum.GetValues(typeof(HazType)).Length];
		for (int i = 0; i < hazardCountArray.Length; i++) {
			hazardCountArray[i] = 0;
		}

		// Add event listener for each hazard.
		// Increment the hazard counts for the hazard's type and for the total number of hazards.
		foreach (Hazard hazard in hazards) {
			hazard.HazardEvent.AddListener(HazardCompleted);
			hazardCountArray[(int)hazard.GetHazType()]++;
			totalHazardCount++;
		}

		// Pass the array of hazard counts to the ObjectivesListUI.
		objectiveList.RecordHazards(hazardCountArray);
	}

	/// <summary>
    /// Listener function that updates the number of hazards completed and updates the ObjectivesListUI.
    /// </summary>
    /// <param name="completed">The new completion status of the hazard.</param>
    /// <param name="hazType">What type of hazard raised the event.</param>
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

	/// <summary>
    /// Called when the player performs an action that would end with their death. Increments deathCount.
    /// </summary>
	public void PlayerDied() {
		deathCount++;
	}

	/// <summary>
    /// Calculates the player's score based on how long they took to complete the level,
    /// how many of the hazards they completed, and how many times they died.
    /// </summary>
    /// <returns> The player's score. </returns>
	public int GetScore () {
		float timeScore = (POINTS_SECONDS_RATIO * (MAX_TIME_SECONDS - gameTimer.getFinalTime()));
		return Mathf.FloorToInt(BASE_SCORE_MULTIPLIER * ((totalHazardCompleted - deathCount) * timeScore) / totalHazardCount);
	}

}
