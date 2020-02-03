using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GameTimer : MonoBehaviour
{
	private Text textClock;
	private bool running = false;
	private Stopwatch timer;

	private void Awake()
    {
        textClock = GetComponent<Text>();
		timer = new Stopwatch();
    }

	private void Start()
	{
		timer.Start();
	}

	private void Update()
    {
		textClock.text = timer.Elapsed.ToString().Substring(0, 8);
    }
}
