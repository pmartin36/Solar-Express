using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LevelSuccessMenu : MonoBehaviour {

	public Image Panel;

	public TMP_Text SuperstarText;
	public TMP_Text PointsUntilNextLevelText;
	public TMP_Text MissionSuccessText;
	public TMP_Text PointCountText;

	public Button[] buttons;

	List<int> cutoffs;
	List<ScoreStar> stars;

	// Use this for initialization
	void OnEnable () {
		Panel = GetComponent<Image>();
		Panel.color = Color.clear;

		MissionSuccessText.color = Color.clear;
		PointCountText.color = Color.clear;

		buttons = GetComponentsInChildren<Button>();

		cutoffs = (GameManager.Instance.ContextManager as LevelManager).PointCutoffs;
		stars = GetComponentsInChildren<ScoreStar>().OrderBy( g => g.transform.position.x ).ToList();
		for(int i = 0; i < stars.Count; i++) {
			stars[i].Init( cutoffs[i] );
		}

		GameManager.Instance.MenuParticles.SetActive(true);

		StartCoroutine(Action());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator Action() {
		float startTime = Time.time;
		float ttime = 1f;

		//dim screen
		while(Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			Panel.color = Color.Lerp(Color.clear, Color.black, jTime);
			yield return new WaitForEndOfFrame();
		}

		//show text before counting
		startTime = Time.time;
		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			Color c = Color.Lerp(Color.clear, Color.white, jTime);

			MissionSuccessText.color = c;
			PointCountText.color = c;
			foreach(ScoreStar s in stars) {
				s.OutsideColor = c;
			}

			yield return new WaitForEndOfFrame();
		}

		//count points and show stars achieved
		int score = (int)(GameManager.Instance.ContextManager as LevelManager).PointManager.Points;
		startTime = Time.time;
		ttime = 5f;
		while (Time.time - startTime < ttime + Time.deltaTime) {
			int countingScore = (int)Mathf.Lerp(0, score, (Time.time - startTime) / ttime);
			PointCountText.text = countingScore.ToString();

			foreach(ScoreStar s in stars) {
				s.CheckIfActive(countingScore);
			}

			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(0.5f);

		//show verbal ranking
		int starActive = stars.Count( s => s.Active );
		if(starActive == 3) {
			SuperstarText.gameObject.SetActive(true);
		}
		else {
			PointsUntilNextLevelText.text = "Only " + (stars[starActive].Cutoff - score) + " points for the next star!";
			PointsUntilNextLevelText.gameObject.SetActive(true);
		}

		//enable buttons
		foreach(Button b in buttons) {
			b.interactable = true;
		}

		//show buttons
		startTime = Time.time;
		ttime = 2f;
		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			Color c = Color.Lerp(Color.clear, Color.white, jTime);
			foreach (Button b in buttons) {
				var colors = b.colors;
				colors.normalColor = c;
				b.colors = colors;
			}
			yield return new WaitForEndOfFrame();
		}
	}
}
