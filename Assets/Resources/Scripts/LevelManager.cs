using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : ContextManager {

	public int LevelNumber;
	public bool LevelStarted;

	public Ship PlayerShip;
	public bool MenuOpen { get; set; }
	public GameObject Menu;

	public PointManager PointManager;
	public ProgressBarManager ProgressBar;
	public Planet StartingPlanet, EndingPlanet;

	public ParticleSystem FingerParticles;

	//if CampaignMode, don't show pt sources
	public bool CampaignMode;
	public bool PlayerDead;

	public LevelFailMenu LevelFail;
	public LevelSuccessMenu LevelSuccess;

	public int TotalAvailablePoints;
	public List<int> PointCutoffs;

	public AudioClip levelAudioClip;

	// Use this for initialization
	public override void Awake() {
		base.Awake();
		GameManager.Instance.RegisterContextManager(this);
	}

	public virtual void Start () {
		MenuOpen = false;		

		if(GameManager.Instance.MusicManager != null) {
			MusicManager m = GameManager.Instance.MusicManager;
			if(m.PlayingSong != levelAudioClip) {
				GameManager.Instance.MusicManager.SetPlayingSong(levelAudioClip);
				GameManager.Instance.MusicManager.SetVolumeLevelGradual(0.3f, 1f);
			}
		}

		StartCoroutine(LoadLevel());
	}
	
	// Update is called once per frame
	protected virtual void Update () {		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (MenuOpen) {
				ToggleMenu();
			}
			else {
				//go to menu
				GameManager.Instance.SwitchLevels(Utils.MenuScene);
			}
		}
	}

	public void ToggleMenu () {
		MenuOpen = !MenuOpen;
		SetMenuState();
	}

	private void SetMenuState() {
		Menu.SetActive(MenuOpen);
		GameManager.Instance.MenuParticles.SetActive(MenuOpen);
		Time.timeScale = MenuOpen ? 0.00001f : 1f;
	}

	IEnumerator LoadLevel() {
		//Image loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<Image>();
		var ps = GameManager.Instance.MenuParticles.GetComponentsInChildren<ParticleSystem>();
		var ssp = GameObject.FindGameObjectWithTag("Scrolling Star Particle").GetComponent<ParticleSystem>();

		//once fade completed, start moving planet
		SetMenuState();

		//wait frame before assigning planets
		yield return new WaitForEndOfFrame();

		//find planets
		var planets = GameObject.FindGameObjectsWithTag("Planet").Select(p => p.GetComponent<Planet>());
		EndingPlanet = planets.First(p => !p.IsStartingPlanet);
		StartingPlanet = planets.First(p => p.IsStartingPlanet);

		StartingPlanet.Moving = true;
		PlayerShip.ChangePlayerSize(Vector3.zero, Vector3.one * 0.75f);

		yield return null;
	}

	//planet has reached offscreen - time to start obstacles
	public virtual void StartLevelSpawn() {
		LevelStarted = true;
	}

	public virtual void BeginEndLevel() {
		if(!PlayerDead) {
			StartCoroutine(EndLevel());
		}
	}

	public virtual void PlayerDied() {

	}

	public IEnumerator PlayerDeathRoutine() {
		//wait for screen shake to stop
		yield return new WaitForSeconds(0.7f);

		//Camera.main.GetComponent<CameraController>().Tremble();

		SpriteRenderer cracks = PlayerShip.Cracks;
		AudioSource crackaudio = cracks.GetComponent<AudioSource>();
		float ttime = 3f;
		float startTime = Time.time;

		crackaudio.Play();
		this.AddAudioSource(crackaudio);
		crackaudio.mute = !GameManager.Instance.PlayerInfo.SoundOn;

		while (Time.time - startTime < ttime) {
			float jTime = (Time.time - startTime) / ttime;
			cracks.material.SetFloat("_Cutoff", 0.6f * jTime);

			if(!LevelFail.gameObject.activeInHierarchy && jTime > 0.95f) {
				LevelFail.gameObject.SetActive(true);
			}

			yield return new WaitForEndOfFrame();
		}

		crackaudio.Stop();

		if (!LevelFail.gameObject.activeInHierarchy) {
			LevelFail.gameObject.SetActive(true);
		}
	}

	IEnumerator EndLevel() {
		EndingPlanet.Moving = true;
		yield return new WaitForSeconds(5f);

		float playerSizeChangeTime = 5f;
		PlayerShip.ChangePlayerSize(Vector2.one * 0.75f, Vector2.zero, playerSizeChangeTime);
		foreach(ParticleSystem ps in PlayerShip.GetComponentsInChildren<ParticleSystem>()) {
			var psem = ps.emission;
			psem.enabled = false;
		}

		yield return new WaitForSeconds(playerSizeChangeTime);
		yield return StartCoroutine(EndingPlanet.Colorize());

		//save score
		bool starAwarded = false;
		for(int i = PointCutoffs.Count - 1; i >= 0; i--) {
			if( PointManager.Points >= PointCutoffs[i]) {
				GameManager.Instance.UpdateLevelStars(LevelNumber, i+1);
				starAwarded = true;
				break;
			}
		}	
		if(!starAwarded) {
			GameManager.Instance.UpdateLevelStars(LevelNumber, 0);
		}

		//pop up score screen
		LevelSuccess.gameObject.SetActive(true);
	}

	public void ProcessInputs(InputPackage p) {
		PlayerShip.Rotate(p);
	}
}
