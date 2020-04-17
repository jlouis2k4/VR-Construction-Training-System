using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;


/// <summary>
/// Class that starts a Stopwatch when instantiated. Time is displayed in HH:MM:SS format.
/// </summary>
public class GameTimer : MonoBehaviour
{
	private Text textClock;
	private bool running = false;
	private Stopwatch timer;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
    {
        textClock = GetComponent<Text>();
		timer = new Stopwatch();
		timer.Start();
	}

	/// <summary>
    /// Update is called once every frame.
    /// </summary>
	private void Update()
    {
		if (String.Compare(textClock.text, "99:99:99") == 0) {
			// Timer maxed out. Should something happen?
		}
		textClock.text = timer.Elapsed.ToString().Substring(0, 8);
    }

	/// <summary>
    /// Stops the Stopwatch and returns the number of seconds elapsed since the Stopwatch started.
    /// </summary>
    /// <returns> Number of seconds elapsed. </returns>
	public int getFinalTime() {
		timer.Stop();
		TimeSpan ts = timer.Elapsed;
		return ts.Seconds + 60 * (ts.Minutes + 60 * ts.Hours);
	}
}
