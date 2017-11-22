using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SettingsInGameButton : InGameButtons {

	public override void Start() {
		base.Start();
	}

	public override void onTouch() {
		(GameManager.Instance.ContextManager as LevelManager).ToggleMenu();
	}
}
