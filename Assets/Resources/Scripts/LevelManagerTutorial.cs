using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class LevelManagerTutorial : LevelManager {

	public TMP_Text TutorialText;
	private bool hitCore, hitShield;
	public Meteor meteorPrefab;

	public override void Start() {
		base.Start();
	}

	public override void StartLevelSpawn() {
		base.StartLevelSpawn();
		StartCoroutine(SpawnLevel());
	}

	IEnumerator FadeInText(float fadetime = 1f) {
		float time = 0f;
		while( time - Time.deltaTime < fadetime ) {
			TutorialText.color = Color.Lerp( Color.clear, Color.white, time / fadetime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator FadeOutText(float fadetime = 1f) {
		float time = 0f;
		while (time - Time.deltaTime < fadetime) {
			TutorialText.color = Color.Lerp(Color.white, Color.clear, time / fadetime);
			time += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator FadeOutChangeTextFadeIn(string newtext) {
		yield return StartCoroutine(FadeOutText());
		yield return new WaitForSeconds(1f);
		TutorialText.text = newtext;
		yield return StartCoroutine(FadeInText());
	}

	IEnumerator SpawnLevel() {
		//move your finger around the screen to rotate the ship
		TutorialText.text = "Move your finger around the screen to rotate the ship";
		yield return StartCoroutine(FadeInText());
		yield return new WaitForSeconds(8f);
		ProgressBar.UpdateProgressBar(0.25f);

		//register for events
		Shield yellowShield = PlayerShip.GetComponentsInChildren<Shield>().First( s => s.GameColor == Colors.Yellow );
		yellowShield.ShieldHit += HitShieldEvent;

		//use your yellow shield to block the yellow meteor
		yield return StartCoroutine(FadeOutChangeTextFadeIn("Use your yellow shield to block the yellow meteor"));
		while (!hitShield) {
			Meteor m = Instantiate(meteorPrefab, new Vector2(5f, 5f), Quaternion.identity);
			m.Init(Colors.Yellow, 45, 3, 0.5f, 0);
			yield return new WaitForSeconds(4f);
		}

		//unregister for events
		yellowShield.ShieldHit -= HitShieldEvent;
		ProgressBar.UpdateProgressBar(0.5f);

		//your ship can get hit once before it explodes
		yield return StartCoroutine(FadeOutChangeTextFadeIn("Your ship can survive one hit but will explode on the second"));
		yield return new WaitForSeconds(2f);

		yellowShield.DisableShieldForTime(5f);

		Meteor m2 = Instantiate(meteorPrefab, new Vector2(5f, 5f), Quaternion.identity);
		m2.Init(Colors.Yellow, 45, 3, 0.5f, 1);

		yield return new WaitForSeconds(4f);
		ProgressBar.UpdateProgressBar(0.75f);

		//good luck
		yield return StartCoroutine(FadeOutChangeTextFadeIn("Good Luck"));
		ProgressBar.UpdateProgressBar(1);
		yield return new WaitForSeconds(1f);
		StartCoroutine(FadeOutText(4f));

		BeginEndLevel();
	}

	public void HitCoreEvent(object sender, EventArgs e) {
		
	}

	public void HitShieldEvent(object sender, EventArgs e) {
		hitShield = true;
	}
}
