using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PointManager : MonoBehaviour {

	public float Points { get; private set; }

	private float currentlyDisplayedPoints = 0;

	private TMP_Text PointSourceDisplay;
	private TMP_Text PointDisplay;

	private bool SettingFontSize = false;
	private float targetTextSize = 35f;
	private bool LargeFontFromPoints = false;

	private int pointColorIndex = 0;

	// Use this for initialization
	void Start () {
		var texts = GetComponentsInChildren<TMP_Text>();
		PointSourceDisplay = texts.First(t => t.tag == "PointSourceDisplay");
		PointDisplay = texts.First(t => t.tag == "PointDisplay");

		StartCoroutine(OscillatePointDisplay());
		StartCoroutine(ColorPoints());
	}
	
	// Update is called once per frame
	void Update () {
		if(!(GameManager.Instance.ContextManager as LevelManager).CampaignMode) {
			Color textColor = PointSourceDisplay.color;
			textColor.a -= Mathf.Clamp01(1f * Time.deltaTime);
			PointSourceDisplay.color = textColor;
		}

		if(!SettingFontSize) {
			PointDisplay.fontSize = Mathf.Max(32, PointDisplay.fontSize - 5f*Time.deltaTime);
		}

		//currentlyDisplayedPoints = Mathf.Min(Points, currentlyDisplayedPoints + 1000 * Time.deltaTime);
		currentlyDisplayedPoints = Mathf.Min(Points, currentlyDisplayedPoints + (Points-currentlyDisplayedPoints+100) * Time.deltaTime);
		PointDisplay.text = currentlyDisplayedPoints.ToString("f0");
	}

	public void IncrementPoints(int points, string source, Color c) {
		Points += points;

		PointDisplay.fontSize = 40f;
		LargeFontFromPoints = true;

		//
		
		if(!(GameManager.Instance.ContextManager as LevelManager).CampaignMode) {
			PointSourceDisplay.color = c;
			PointSourceDisplay.text = source;
		}
	}

	IEnumerator OscillatePointDisplay() {
		while(true) {
			SettingFontSize = true;
			while(PointDisplay.fontSize < targetTextSize) {
				PointDisplay.fontSize += LargeFontFromPoints ? 10f * Time.deltaTime : 5f*Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			SettingFontSize = false;
			yield return new WaitUntil(() => PointDisplay.fontSize < 32.01);
		}
	}

	IEnumerator ColorPoints() {
		while(true) {
			while( Points - currentlyDisplayedPoints > 5  ) {
				PointDisplay.color = new Color(Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1);
				yield return new WaitForSeconds(0.1f);
			}
			PointDisplay.color = Color.white;
			LargeFontFromPoints = false;
			yield return new WaitUntil( () => LargeFontFromPoints );
		}
	}
}
