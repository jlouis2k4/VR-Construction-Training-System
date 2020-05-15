using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enumerator defining the type of a hazard
/// </summary>
public enum HazType {
	Crushing=0,
	Electric=1,
	Falling=2,
	Fire=3,
	Other=4
}

/// <summary>
/// Unity Event that raises a flag when the completeion status of a hazard changes.
/// </summary>
[System.Serializable]
public class HazardCompletedEvent : UnityEvent<bool, HazType> {
}

/// <summary>
/// Class representing a workplace hazard.
/// </summary>
public class Hazard : MonoBehaviour
{
	[HideInInspector] public HazardCompletedEvent HazardEvent = new HazardCompletedEvent();

	private bool isCompleted = false;

    /// <summary>
    /// The completion status of this hazard.
    /// </summary>
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
	
	/// <summary>
    /// Returns this hazard's type.
    /// </summary>
    /// <returns>The HazType Enum of this Hazard</returns>
	public HazType GetHazType() {
		return type;
	}
}
