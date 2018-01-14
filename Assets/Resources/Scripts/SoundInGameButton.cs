using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInGameButton : InGameButtons {

	public static Sprite SoundOnSprite;
	public static Sprite SoundOffSprite;

	public override void Start() {
		base.Start();

		SoundOffSprite = SoundOffSprite ?? Resources.Load<Sprite>("Sprites/SoundOff");
		SoundOnSprite = SoundOnSprite ?? Resources.Load<Sprite>("Sprites/SoundOn");

		SetSprite();
	}

	private void SetSprite() {
		image.overrideSprite = GameManager.Instance.PlayerInfo.SoundOn ? SoundOnSprite : SoundOffSprite;
	}

	public override void onTouch() {
		GameManager.Instance.ToggleSoundOn();
		SetSprite();
	}
}
