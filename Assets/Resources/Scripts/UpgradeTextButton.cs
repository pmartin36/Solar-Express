using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTextButton : MonoBehaviour {

	private Color[] colors;
	Material m;

	private int colorIndex = 0;
	private Color startColor, targetColor;

	// Use this for initialization
	void Start () {
		colors = new Color[] {
			Color.red,
			new Color(1f, 0.5f, 0), //orange
			Color.yellow,
			Color.green,
			Color.blue,
			new Color(0.3f, 0, 0.51f), //indigo
			new Color(0.54f, 0.17f, 0.89f) //violet
		};

		m = GetComponent<TMPro.TMP_Text>().fontMaterial;
		StartCoroutine(RotateColors());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void SetUnderlayColor(Color c) {
		m.SetColor("_UnderlayColor", c);
	}

	private void SetTargetColor() {
		colorIndex = ++colorIndex % colors.Length;
		targetColor = colors[colorIndex];
	}

	IEnumerator RotateColors() {
		float ttime = 3f;
		startColor = colors[0];

		while (true) {
			float startTime = Time.time;
			SetTargetColor();

			while (Time.time - startTime < ttime + Time.deltaTime) {
				SetUnderlayColor(Color.Lerp(startColor, targetColor, (Time.time - startTime)/ttime));
				yield return new WaitForEndOfFrame();
			}

			startColor = targetColor;
		}
	}
}
