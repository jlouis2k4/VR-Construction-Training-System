using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class GameTimer : MonoBehaviour
{
	private Text textClock;
	private bool running = false;
	private Stopwatch timer;

	private void Awake()
    {
        textClock = GetComponent<Text>();
		timer = new Stopwatch();
		timer.Start();
	}

	public void StartTimer()
	{
		timer.Start();
	}

	private void Update()
    {
		if (String.Compare(textClock.text, "99:99") == 0) {
			// Timer maxed out. Should the level stop?
		}
		textClock.text = timer.Elapsed.ToString().Substring(0, 8);
    }

	public int getFinalTime() {
		timer.Stop();
		TimeSpan ts = timer.Elapsed;
		return ts.Seconds + 60 * (ts.Minutes + 60 * ts.Hours);
	}
}
