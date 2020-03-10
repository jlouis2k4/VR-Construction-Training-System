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
    public bool Completed {
        get {
            return isCompleted;
        }
        set {
            isCompleted = value;
            HazardEvent.Invoke(value, type);
        }
    }

    public HazType type = HazType.Other;
	
	public HazType GetHazType() {
		return type;
	}

}
