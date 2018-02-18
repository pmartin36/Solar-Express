using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : ContextManager {

	// Use this for initialization
	void Start () {
		GameManager.Instance.ContextManager = this;
		GetComponent<AudioSource>().mute = !GameManager.Instance.PlayerInfo.SoundOn;

		Camera cam = Camera.main;
		float height = cam.orthographicSize;
		float width = height * cam.aspect;
		if (width < height) {
			cam.orthographicSize = 3.5f / cam.aspect;
		}

	}
	
	public void GoToMenu() {
		GameManager.Instance.SwitchLevels(Utils.MenuScene);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
