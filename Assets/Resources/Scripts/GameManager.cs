using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	public CameraController MainCameraController { get; set; }
	public ContextManager ContextManager { get; set; }
	public bool TransitioningToHome { get; set; }

	public PlayerInfo PlayerInfo { get; set; }
	public GameObject MenuParticles { get; set; }

	public void Awake() {		
		TransitioningToHome = true;

		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		PlayerInfo = Serializer<PlayerInfo>.Deserialize("playerinfo.bin");
		PlayerInfo = PlayerInfo ?? new PlayerInfo();

		//for testing
		PlayerInfo.LevelStars = new List<int>() {
			1, 2, 3, 2, 1, 0, 3, 2, 1
		};

		GameObject particlePrefab = Resources.Load<GameObject>("Prefabs/MenuParticles");
		MenuParticles = Instantiate(particlePrefab);
		DontDestroyOnLoad(MenuParticles);
	}

	public void SwitchLevels(int index = 0) {
		Time.timeScale = 1f;
		SceneManager.LoadScene(index);
	}

	public void SaveGame() {
		Serializer<PlayerInfo>.Serialize(PlayerInfo, "playerinfo.bin");
	}


	public void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void RegisterContextManager(ContextManager ctm) {
		ContextManager = ctm;

		if(ctm is MenuManager) {
			(ctm as MenuManager).SetActiveScreen(TransitioningToHome);		
		}
		TransitioningToHome = false;
	}

	public void UpdateLevelStars(int levelNumber, int stars) {
		if(PlayerInfo.LevelStars.Count <= levelNumber) {
			PlayerInfo.LevelStars.Add(stars);
		}
		else {
			PlayerInfo.LevelStars[levelNumber] = Mathf.Max(stars, PlayerInfo.LevelStars[levelNumber]);
		}
		SaveGame();
	}
}