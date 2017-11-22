using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	public CameraController MainCameraController { get; set; }
	public ContextManager ContextManager { get; set; }
	public bool SoundOn { get; set; }
	public bool TransitioningToHome { get; set; }

	public void Awake() {
		SoundOn = true;	
		TransitioningToHome = true;
	}

	public void SwitchLevels(int index = 0) {
		Time.timeScale = 1f;
		SceneManager.LoadScene(index);
	}

	public void RegisterContextManager(ContextManager ctm) {
		ContextManager = ctm;

		if(ctm is MenuManager) {
			(ctm as MenuManager).SetActiveScreen(TransitioningToHome);		
		}
		TransitioningToHome = false;
	}
}