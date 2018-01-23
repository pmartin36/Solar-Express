using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingText : MonoBehaviour {

	public bool Ascending { get; set; }
	public float AlphaValue { get; set; }
	TMPro.TMP_Text text;

	Color[] colors;
	int currentcolorindex = 0;
	int nextcolorindex = 1;
	float colorVal;

	// Use this for initialization
	void OnEnable () {
		Ascending = true;
		AlphaValue = 0f;
		colorVal = 0f;
		text = GetComponent<TMPro.TMP_Text>();

		colors = new Color[] {
			Color.red,
			new Color(1, 0.5f, 0),
			Color.yellow,
			Color.green,
			Color.blue,
			new Color(0.3f, 0, 0.5f),
			new Color(0.33f, 0.1f, 0.55f)
		};
	}
	
	// Update is called once per frame
	void Update () {
		Color c = Color.Lerp(colors[currentcolorindex], colors[nextcolorindex], colorVal);
		c.a = AlphaValue;
		text.color = c;

		colorVal = (colorVal + Time.deltaTime * 3);
		if(colorVal >= 1) {
			colorVal -= 1;
			currentcolorindex = nextcolorindex;
			nextcolorindex = (currentcolorindex + 1) % colors.Length;
		}

		float valToAdd = Ascending ? Time.deltaTime : -Time.deltaTime;
		AlphaValue += valToAdd;

		if(AlphaValue < 0 || AlphaValue > 1) {
			Ascending = !Ascending;
			AlphaValue = Mathf.Clamp01(AlphaValue);
		}
	}
}
