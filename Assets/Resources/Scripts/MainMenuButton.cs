using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour {

	public enum MainMenuButtonType {
		//Home Screen
		Play,
		Upgrade,
		Credits,
		//Level Select Screen
		Continue,
		Beamium,
		Endless,
		Exit,
		//Story Screen
		Next
	}

	public MainMenuButtonType ButtonType;
	Color targetColor, startColor;
	Button button;
	TMP_Text text;

	public Popup Popup1, Popup2;

	[SerializeField]
	private bool _enabled;
	public bool Enabled {
		get {
			return _enabled;
		}
		private set {
			_enabled = value;
			text.fontMaterial = Enabled ? EnabledMaterial : DisabledMaterial;
		}
	}
	public Material EnabledMaterial, DisabledMaterial;

	// Use this for initialization
	void Start () {
		if( ButtonType == MainMenuButtonType.Continue && (GameManager.Instance.PlayerInfo.LevelStars.Count < 1)) {
			text.text = "Start";
		}		
	}

	private void OnEnable() {
		text = GetComponent<TMP_Text>();
		button = GetComponent<Button>();

		text.fontMaterial = Enabled ? EnabledMaterial : DisabledMaterial;
		StartCoroutine(RotateColors());
	}

	public void onTouch() {
		switch (ButtonType) {
			default:
			case MainMenuButtonType.Play:
				(GameManager.Instance.ContextManager as MenuManager).SwitchToLevelSelect();
				break;
			case MainMenuButtonType.Upgrade:
				break;
			case MainMenuButtonType.Credits:
				(GameManager.Instance.ContextManager as MenuManager).OpenPopup(Popup1);
				break;
			case MainMenuButtonType.Continue:
				(GameManager.Instance.ContextManager as MenuManager).CloseMenuStartPlay(GameModes.Campaign);
				break;
			case MainMenuButtonType.Beamium:
				break;
			case MainMenuButtonType.Endless:
				Debug.Log((GameManager.Instance.ContextManager as MenuManager).LevelSelector.Selected);
				break;
			case MainMenuButtonType.Exit:
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetTargetColor() {
		targetColor = Enabled ? new Color(Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, Random.value * 0.5f + 0.5f, 1) : Color.gray;
	}

	IEnumerator RotateColors() {
		float ttime = 1f;
		while (true) {
			float startTime = Time.time;
			SetTargetColor();
			while (Time.time - startTime < ttime) {
				float jTime = (Time.time - startTime) / ttime;
				text.color = Color.Lerp( startColor, targetColor, jTime);
				yield return new WaitForEndOfFrame();
			}
			startColor = targetColor;
		}
	}
}
