using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuInGameButton : InGameButtons {

	public enum InGameMenuButtons {
		Resume,
		Restart,
		LevelSelect,
		Menu,
		NextLevel
	}

	public InGameMenuButtons buttonType;
	private TMP_Text text;

	private Color startColor, targetColor;

	private void Awake() {
		startColor = new Color(Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1);	
	}

	public override void Start () {
		base.Start();	
	}

	private void OnEnable() {
		text = GetComponent<TMP_Text>();
		StartCoroutine(RotateColors());
	}

	private void OnDisable() {
		StopCoroutine(RotateColors());
	}

	public override void onTouch() {
		base.onTouch();
		switch (buttonType) {
			default:
			case InGameMenuButtons.Resume:
				(GameManager.Instance.ContextManager as LevelManager).ToggleMenu();
				break;
			case InGameMenuButtons.Restart:
				Time.timeScale = 1f;
				GameManager.Instance.ReloadLevel();
				break;
			case InGameMenuButtons.LevelSelect:
				GameManager.Instance.TransitioningToHome = false;
				GameManager.Instance.SwitchLevels(Utils.MenuScene);
				break;
			case InGameMenuButtons.Menu:
				GameManager.Instance.TransitioningToHome = true;
				GameManager.Instance.SwitchLevels(Utils.MenuScene);
				break;
			case InGameMenuButtons.NextLevel:
				// we don't need the +1 because the scene for a level number is added in Utils.LoadSceneFromLevel
				GameManager gm = GameManager.Instance;
				gm.PlayerInfo.LevelSelectIndex++;
				gm.SwitchLevels(Utils.LevelSceneFromLevel((gm.ContextManager as LevelManager).LevelNumber));
				break;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	void SetTargetColor() {
		targetColor = new Color(Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1);
	}

	IEnumerator RotateColors() {
		float ttime = 1f;
		float interval = 1/60f;

		while (true) {
			float elapsedTime = 0f;
			SetTargetColor();
			while (elapsedTime < ttime) {
				text.color = Color.Lerp(startColor, targetColor, elapsedTime);
				elapsedTime += interval;

				yield return new WaitForSecondsRealtime(interval);
			}
			startColor = targetColor;
		}
	}
}
