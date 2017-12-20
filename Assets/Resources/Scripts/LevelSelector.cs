using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

public class LevelSelector : ScrollRect {

	public float SnapSize;

	public float singleElementWidth;
	public float totalWidth;
	public int numTiles;

	public float prevval;

	float releaseTime, releaseVelocity, releasePosition;
	float finalPosition;
	bool moving;

	public float decelRate = 0.135f;
	public List<LevelSelectElement> Elements;

	public int Selected {
		get {
			return (int) Math.Round( this.horizontalNormalizedPosition * (numTiles) );
		}
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();
		if(!EditorApplication.isPlaying) return;

		Elements = content.GetComponentsInChildren<LevelSelectElement>().OrderBy(g => g.transform.position.x).ToList();
		numTiles = GameManager.Instance.PlayerInfo.LevelStars.Count;
		for (int i = 0; i < Elements.Count; i++) {
			LevelSelectElement e = Elements[i];
			if ( i <= numTiles) {
				var ls = GameManager.Instance.PlayerInfo.LevelStars;
				e.gameObject.SetActive(true);
				e.SetStars( ls.Count > i ? ls[i] : 0 );
			}
			else {
				e.gameObject.SetActive(false);
			}		
		}

		singleElementWidth = Elements[0].minWidth;
		totalWidth = (numTiles+1) * singleElementWidth;

		this.horizontalNormalizedPosition = 1;
		
		/*
		var layoutelements = content.GetComponentsInChildren<LayoutElement>();
		numTiles = layoutelements.Length - 1;
		if (layoutelements.Length > 0) {
			singleElementWidth = layoutelements[0].minWidth;
			totalWidth = (layoutelements.Length) * singleElementWidth;
		}
		*/
	}

	protected override void OnEnable() {
		base.OnEnable();
	}

	// Update is called once per frame
	void Update () {
		if (moving) {
			//DEBUGGING
			float timeElapsed = (Time.time - releaseTime);
			//Debug.Log("--- " + timeElapsed + " ---");
			//Debug.Log("Amount Moved: " + (this.horizontalNormalizedPosition - releasePosition) * totalWidth * 0.9);
			//Debug.Log("Estimated Amount Moved: " + releaseVelocity * (0.434294f - 0.434294f * Mathf.Pow(0.1f, timeElapsed)));
			//this.horizontalNormalizedPosition = releasePosition + (releaseVelocity * (0.434294f - 0.434294f * Mathf.Pow(0.1f, timeElapsed))) / (totalWidth * 0.9f);
		}
	}

	private void FixedUpdate() {
		if(moving) {
		
			//MAY BE NECESSARY FOR FIXES
			float movingTime = Time.time - releaseTime;
			//velocity = new Vector2(releaseVelocity * Mathf.Pow(decelerationRate, movingTime), 0);

			//DEBUGGING
			if (Mathf.Abs(velocity.x) <= 100f) {			
				float factor = (float)numTiles / (numTiles+1);
				float fix = (this.horizontalNormalizedPosition - finalPosition) * totalWidth * factor;
				if( Mathf.Abs(fix) < 1f ) {
					moving = false;
					Debug.Log("Final Position: " + this.horizontalNormalizedPosition * totalWidth * factor);//numTiles);
					Debug.Log(new string('-', 20));
				}
				else {
					velocity = new Vector2(fix*4f,0);
				}
			}
		}
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag(eventData);

		//DEBUGGING
		releaseTime = Time.time;
		releaseVelocity = velocity.x;
		releasePosition = this.horizontalNormalizedPosition;
		moving = true;
		float incomingvelocity = velocity.x;
		Debug.Log("Incoming Velocity: " + incomingvelocity);

		//CORE LOGIC		
		// V(t) = Vo * (decayRate)^(t)
		// Integral of Vo*decayRate^t = Distance Travelled = -Vo / log(decayRate)

		float alpha = Mathf.Log(decelerationRate);
		float factor = (float)numTiles / (numTiles + 1);
		float startPosition = this.horizontalNormalizedPosition * totalWidth * factor;
		float movement = (velocity.x / alpha);
		float estPos =  startPosition + movement;
		float nearestTile = Mathf.Round(estPos / singleElementWidth) * singleElementWidth;
		velocity = new Vector2( (nearestTile - startPosition) * (alpha), 0);

		finalPosition = nearestTile / (totalWidth * factor);

		//DEBUGGING
		Debug.Log("Position: " + startPosition);
		Debug.Log("Incoming Velocity: " + incomingvelocity);
		Debug.Log("Initial Velocity: " + velocity.x);
		Debug.Log("Estimated Movement: " + movement);
		Debug.Log("Initial Estimated Position: " + (estPos));
		Debug.Log("Tiled Position: " + (nearestTile) );
	}

	public void SetSelectedRingAlpha(float v) {
		Elements[Selected].SetRingAlpha(v);
	}

	public void SetElementsAlpha2(float alpha) {
		foreach(LevelSelectElement e in Elements) {
			e.SetAlpha2(alpha);
		}
	}

	public void SetElementsAlpha(float alpha) {
		foreach (LevelSelectElement e in Elements) {
			e.SetAlpha(alpha);
		}
	}
}
