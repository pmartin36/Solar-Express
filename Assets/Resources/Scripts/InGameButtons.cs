using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameButtons : MonoBehaviour {

	Color targetColor, startColor;
	float startTime;
	protected Image image;

	// Use this for initialization
	public virtual void Start () {
		startColor = Color.white;
		image = GetComponent<Image>();
		StartCoroutine(RotateColors());
	}
	
	void SetTargetColor() {
		targetColor = new Color( Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1);
	}

	public virtual void onTouch() {

	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator RotateColors() {
		float ttime = 2f;
		while (true) {
			float startTime = Time.time;
			SetTargetColor();
			while( Time.time - startTime < ttime) {
				float jTime = (Time.time - startTime) / ttime;
				//image.color = Color.Lerp( startColor, targetColor, jTime);
				yield return new WaitForEndOfFrame();
			}
			startColor = targetColor;
		}
	}
}
