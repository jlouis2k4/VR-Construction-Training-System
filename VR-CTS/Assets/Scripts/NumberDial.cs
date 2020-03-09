using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDial : MonoBehaviour
{
	private int value;
	public Text counterText;

	private void Awake()
	{
		counterText.text = value.ToString();
	}

	public void UpArrowClick() {
		if (value == 9) value = 0;
		else value++;
		counterText.text = value.ToString();
	}

	public void DownArrowClick()
	{
		if (value == 0) value = 9;
		else value--;
		counterText.text = value.ToString();
	}

	public int getValue() {
		return value;
	}
}
