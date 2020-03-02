using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum HazType {
	Crushing=0,
	Electric=1,
	Falling=2,
	Fire=3,
	Other=4
}

[System.Serializable]
public class HazardCompletedEvent : UnityEvent<bool, HazType> {
}

public class Hazard : MonoBehaviour
{
	[HideInInspector] public HazardCompletedEvent HazardEvent = new HazardCompletedEvent();

	private bool isCompleted = false;
	public HazType type = HazType.Other;
	
	public HazType GetHazType() {
		return type;
	}

	public bool HasBeenCompleted() {
		return isCompleted;
	}

	public void SetCompletionStatus(bool completed) {
		isCompleted = completed;
		HazardEvent.Invoke(completed, type);
	}

}
