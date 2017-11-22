using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PointManager : MonoBehaviour {

	public float Points { get; private set; }
	private TMP_Text PointSourceDisplay;
	private TMP_Text PointDisplay;

	// Use this for initialization
	void Start () {
		var texts = GetComponentsInChildren<TMP_Text>();
		PointSourceDisplay = texts.First(t => t.tag == "PointSourceDisplay");
		PointDisplay = texts.First(t => t.tag == "PointDisplay");
	}
	
	// Update is called once per frame
	void Update () {
		Color textColor = PointSourceDisplay.color;
		textColor.a -= Mathf.Clamp01(1f * Time.deltaTime);
		PointSourceDisplay.color = textColor;
	}

	public void IncrementPoints(int points, string source, Color c) {
		Points += points;
		PointDisplay.text = Points.ToString();
		
		PointSourceDisplay.color = c;

		PointSourceDisplay.text = source;
	}
}
