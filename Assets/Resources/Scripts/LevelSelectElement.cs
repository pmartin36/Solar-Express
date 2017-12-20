using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelSelectElement : LayoutElement {

	private List<Image> stars;
	public static Sprite filledStar;

	private TMP_Text LevelName;
	private Image LevelImage;
	private Image LevelImageRing;

	// Use this for initialization
	protected override void Start () {
		InitStars();
		LevelName = GetComponentInChildren<TMP_Text>();
		LevelImage = GetComponentsInChildren<Image>().First( g => g.tag == "Planet");
		LevelImageRing = GetComponentsInChildren<Image>().FirstOrDefault(g => g.tag == "PlanetRing");
	}
	
	// Update is called once per frame
	protected void Update () {
		
	}

	void InitStars() {
		filledStar = filledStar ?? Resources.Load<Sprite>("Sprites/star_fill");
		stars = stars ?? GetComponentsInChildren<Image>().Where(g => g.tag == "ScoreStar").OrderBy(g => g.transform.position.x).ToList();
	}

	private void SetStarFilled(Image star) {
		star.sprite = filledStar;
		star.transform.localScale = Vector3.one * 1.45f;
	}

	public void SetStars(int numStars) {
		InitStars();
		for(int i = 0; i < numStars; i++) {
			SetStarFilled(stars[i]);
		}
	}

	public void SetAlpha(float alpha) {
		if( !gameObject.activeInHierarchy ) return;

		Color c = new Color( 1, 1, 1, alpha );
		foreach(Image s in stars) {
			s.color = c;
		}

		LevelName.color = c;
		LevelImage.color = c;
	}

	public void SetAlpha2(float alpha) {
		if (!gameObject.activeInHierarchy) return;

		Color c = new Color(1, 1, 1, alpha);
		foreach (Image s in stars) {
			s.color = c;
		}

		LevelName.color = c;
	}

	public void SetRingAlpha(float v) {
		if(LevelImageRing == null) return;

		Color c = LevelImageRing.color;
		c.a = v;
		LevelImageRing.color = c;
	}
}
