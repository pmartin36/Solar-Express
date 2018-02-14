using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RateButton : MonoBehaviour {

	private TMP_Text text;
	private Button button;

	// Use this for initialization
	void OnEnable () {
		text = GetComponentInChildren<TMP_Text>();
		button = GetComponent<Button>();
		StartCoroutine(RotateColors());
	}
	
	// Update is called once per frame
	void OnDisable () {
		StopCoroutine(RotateColors());
	}

	IEnumerator RotateColors() {
		float ttime = 0.5f;
		TMP_Text text = GetComponentInChildren<TMP_Text>();

		Color startColor, targetColor;		
		startColor = Color.red;

		while (true) {
			float startTime = Time.time;
			targetColor = new Color(Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1);
			while (Time.time - startTime < ttime) {
				float jTime = (Time.time - startTime) / ttime;
				//text.material.SetColor("_UnderlayColor", Color.Lerp(startColor, targetColor, jTime));
				text.color = Color.Lerp(startColor, targetColor, jTime);
				yield return new WaitForEndOfFrame();
			}
			startColor = targetColor;
		}
	}

	public void RateClicked() {
		if( GameManager.Instance.ContextManager is MenuManager ) {
			(GameManager.Instance.ContextManager as MenuManager).GoToStore();
		}
	}
}
