using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressBarManager : MonoBehaviour {

	RectTransform ship, bar;

	float max, min;

	// Use this for initialization
	void Start () {
		var rects = GetComponentsInChildren<RectTransform>();
		ship = rects.First( r => r.tag == "ProgressBarShip");
		bar = rects.First(r => r.tag == "ProgressBar");

		UpdateProgressBar(0);
	}
	
	public void UpdateProgressBar(float pct) {		
		max = bar.sizeDelta.x / 2f;
		min = max * -1f;

		var rect = ship.rect;
		ship.anchoredPosition = new Vector2(Mathf.Lerp(min, max, pct), ship.anchoredPosition.y);
		//ship.position = new Vector2( Mathf.Lerp(min, max, pct), ship.position.y);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
