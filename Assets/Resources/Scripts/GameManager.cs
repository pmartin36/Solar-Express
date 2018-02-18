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
	public bool ShouldShowRatingPlea { get; set; }

	public PlayerInfo PlayerInfo { get; set; }
	public GameObject MenuParticles { get; set; }
	public MusicManager MusicManager { get; set; }

	public Dictionary<int, List<IThreatParams>> ThreatParams;

	public bool FirstTimePlaying { get; set; }
	private AsyncOperation AsyncSceneLoading;

	public void Awake() {
		TransitioningToHome = true;

		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		PlayerInfo = Serializer<PlayerInfo>.Deserialize("playerinfo.bin");
		PlayerInfo = PlayerInfo ?? new PlayerInfo();

		ThreatParams = Serializer<Dictionary<int, List<IThreatParams>>>.DeserializeLocal("Threats.bytes");

		/*  for testing */
		//PlayerInfo.LevelStars = new List<int>() {
		//	1, 2, 3, 2, 1, 0
		//};

		//PlayerInfo.LevelStars = new List<int>();

		//ThreatParams = Serializer<Dictionary<int, List<IThreatParams>>>.Deserialize("Threats.bin");
		if(ThreatParams != null) {
			string levels = "";
			foreach(var k in ThreatParams) {
				levels += k.Key + ", ";
			}
			Debug.Log(levels);
		}
		else {
			Debug.Log("No threat params yet");
			ThreatParams = new Dictionary<int, List<IThreatParams>>();
		}
		PlayerInfo.LevelStars = PlayerInfo.LevelStars.Take(ThreatParams.Count+1).ToList();

		/* end testing */

		GameObject particlePrefab = Resources.Load<GameObject>("Prefabs/MenuParticles");
		MenuParticles = Instantiate(particlePrefab);
		DontDestroyOnLoad(MenuParticles);

		MusicManager = new GameObject("Music").AddComponent<MusicManager>().Init();
		DontDestroyOnLoad(MusicManager);
		//MusicManager.SetPlayingSong( Resources.Load<AudioClip>("Music/anttisinstrumentals+sytrusy") );

		FirstTimePlaying = PlayerInfo.LevelStars.Count == 0;

		Screen.orientation = ScreenOrientation.Portrait;
		Screen.autorotateToLandscapeLeft = Screen.autorotateToLandscapeRight = false;

		ShouldPlayAudioSources(PlayerInfo.SoundOn);
	}

	public bool SwitchLevels(int index = 0) {
		Time.timeScale = 1f;
		SaveGame(); //save game on scene transition

		if(SceneManager.sceneCountInBuildSettings > index) {
			SceneManager.LoadSceneAsync(index);
			return true;
		}
		return false;
	}

	public void SwitchLevels(int index, Func<bool> predicate) {
		Time.timeScale = 1f;
		SaveGame();
		StartCoroutine(WaitForPredicateToSwitchScene(index, predicate));
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
		levelNumber--;
		if(levelNumber >= PlayerInfo.LevelStars.Count) {
			PlayerInfo.LevelStars.Add(stars);
		}
		else {
			PlayerInfo.LevelStars[levelNumber] = Mathf.Max(stars, PlayerInfo.LevelStars[levelNumber]);
		}
		SaveGame();
	}

	IEnumerator WaitForPredicateToSwitchScene(int index, Func<bool> p) {
		AsyncSceneLoading = SceneManager.LoadSceneAsync(index);
		AsyncSceneLoading.allowSceneActivation = false;
		yield return new WaitUntil(() => AsyncSceneLoading.progress >= 0.9f); //when allowsceneactive is false, progress stops at .9f
		yield return new WaitUntil(p);
		AsyncSceneLoading.allowSceneActivation = true;
	}
}