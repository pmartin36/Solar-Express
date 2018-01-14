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
	public MusicManager MusicManager { get; set; }

	public bool FirstTimePlaying { get; set; }

	public void Awake() {		
		TransitioningToHome = true;

		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		PlayerInfo = Serializer<PlayerInfo>.Deserialize("playerinfo.bin");
		PlayerInfo = PlayerInfo ?? new PlayerInfo();

		//for testing

		//PlayerInfo.LevelStars = new List<int>() {
		//	1, 2, 3, 2, 1, 0
		//};

		PlayerInfo.LevelStars = new List<int>();

		GameObject particlePrefab = Resources.Load<GameObject>("Prefabs/MenuParticles");
		MenuParticles = Instantiate(particlePrefab);
		DontDestroyOnLoad(MenuParticles);

		MusicManager = new GameObject("Music").AddComponent<MusicManager>().Init();
		DontDestroyOnLoad(MusicManager);
		MusicManager.SetPlayingSong( Resources.Load<AudioClip>("Music/anttisinstrumentals+sytrusy") );

		FirstTimePlaying = PlayerInfo.LevelStars.Count == 0;

		ShouldPlayAudioSources(PlayerInfo.SoundOn);
	}

	public void SwitchLevels(int index = 0) {
		Time.timeScale = 1f;
		SaveGame(); //save game on scene transition
		SceneManager.LoadScene(index);
	}

	public void SaveGame() {
		Serializer<PlayerInfo>.Serialize(PlayerInfo, "playerinfo.bin");
	}


	public void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void ToggleSoundOn() {
		this.PlayerInfo.SoundOn = !this.PlayerInfo.SoundOn;
		ShouldPlayAudioSources(this.PlayerInfo.SoundOn);
	}

	public void ShouldPlayAudioSources(bool soundon) {
		MusicManager.Mute = !soundon;
		if(soundon) {
			if(ContextManager != null)
				ContextManager.UnmuteAudioSources();
		}
		else {
			if (ContextManager != null)
				ContextManager.MuteAudioSources();
		}
	}

	public void RegisterContextManager(ContextManager ctm) {
		ContextManager = ctm;

		if(ctm is MenuManager) {
			(ctm as MenuManager).SetActiveScreen(TransitioningToHome);		
		}
		TransitioningToHome = false;
	}

	public void UpdateLevelStars(int levelNumber, int stars) {
		FirstTimePlaying = false;
		if(PlayerInfo.LevelStars.Count <= levelNumber) {
			PlayerInfo.LevelStars.Add(stars);
		}
		else {
			PlayerInfo.LevelStars[levelNumber] = Mathf.Max(stars, PlayerInfo.LevelStars[levelNumber]);
		}
		SaveGame();
	}
}