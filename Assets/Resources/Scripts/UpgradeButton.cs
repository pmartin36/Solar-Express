using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {

	public float colorLerp;
	private TMP_Text text;
	private Button button;

	// Use this for initialization
	void OnEnable () {
		//StartCoroutine(RotateColors());
	}
	
	// Update is called once per frame
	void OnDisable () {
		//StopCoroutine(RotateColors());
	}

	private void Start() {
		text = GetComponentInChildren<TMP_Text>();
		button = GetComponent<Button>();
	}

	private void Update() {
		//text.color = Color.Lerp(new Color(0.5f, 0.5f, 0.5f), Color.white, colorLerp);

		var colors = button.colors;
		colors.normalColor = Color.Lerp(new Color(0.7f, 0.7f, 0.7f), Color.white, colorLerp);
		button.colors = colors;
	}

	IEnumerator RotateColors() {
		float ttime = 4f;
		TMP_Text text = GetComponentInChildren<TMP_Text>();

		Color startColor, targetColor;		
		startColor = Color.black;

		while (true) {
			float startTime = Time.time;
			targetColor = startColor == Color.black ? new Color(0.8f, 0.8f, 0.8f) : Color.black;
			while (Time.time - startTime < ttime) {
				float jTime = (Time.time - startTime) / ttime;
				text.color = Color.Lerp(startColor, targetColor, jTime);
				yield return new WaitForEndOfFrame();
			}
			startColor = targetColor;
		}
	}
}
