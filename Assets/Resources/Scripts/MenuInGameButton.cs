using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInGameButton : InGameButtons {

	public enum InGameMenuButtons {
		Resume,
		Restart,
		LevelSelect,
		Menu
	}

	public InGameMenuButtons buttonType;

	// Use this for initialization
	public override void Start () {
		base.Start();
	}

	public override void onTouch() {
		base.onTouch();
		switch (buttonType) {
			default:
			case InGameMenuButtons.Resume:
				(GameManager.Instance.ContextManager as LevelManager).ToggleMenu();
				break;
			case InGameMenuButtons.Restart:
				break;
			case InGameMenuButtons.LevelSelect:
				GameManager.Instance.TransitioningToHome = false;
				GameManager.Instance.SwitchLevels(1);
				break;
			case InGameMenuButtons.Menu:
				GameManager.Instance.TransitioningToHome = true;
				GameManager.Instance.SwitchLevels(1);
				break;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
