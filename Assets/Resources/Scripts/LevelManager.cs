using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : ContextManager {

	public int LevelNumber;

	public LaserShip enemyShipPrefab;
	public Meteor meteorPrefab;
	public OrbitingEnemy orbiterPrefab;
	public EMP EMPPrefab;
	public PointBeam pointBeamPrefab;

	public Ship PlayerShip;
	public bool MenuOpen { get; set; }
	public GameObject Menu;

	public PointManager PointManager;
	public ProgressBarManager ProgressBar;
	public Planet StartingPlanet, EndingPlanet;

	public ParticleSystem FingerParticles;

	private float LevelLength = 125f;
	private float CurrentLevelTime = 0f;

	//if CampaignMode, don't show pt sources
	public bool CampaignMode;
	public bool PlayerDead;

	public LevelFailMenu LevelFail;
	public LevelSuccessMenu LevelSuccess;

	public List<int> PointCutoffs;

	public AudioClip levelAudioClip;

	// Use this for initialization
	public override void Awake() {
		base.Awake();
		GameManager.Instance.RegisterContextManager(this);
	}

	void Start () {
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
	void Update () {		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (MenuOpen) {
				ToggleMenu();
			}
			else {
				//go to menu
				GameManager.Instance.SwitchLevels(Utils.MenuScene);
			}
		}

		if (!PlayerDead) {
			CurrentLevelTime += Time.deltaTime;
			ProgressBar.UpdateProgressBar(CurrentLevelTime / LevelLength);
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

		//spawn some particles
		//GameManager.Instance.MenuParticles.SetActive(true);
		//var sspem = ssp.emission;
		//sspem.enabled = false;

		//yield return new WaitForEndOfFrame();
		

		//yield return new WaitForSeconds(3f);

		//reenable emission for menu particles
		//sspem.enabled = true;
		//foreach (ParticleSystem p in ps) {
		//	var em = p.emission;
		//	em.enabled = true;
		//}

		//yield return new WaitForEndOfFrame();

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
	public void StartSpawningEnemies() {
		StartCoroutine(SpawnLevel());
	}

	public void BeginEndLevel() {
		if(!PlayerDead) {
			StartCoroutine(EndLevel());
		}
	}

	public void PlayerDied() {
		PlayerDead = true;
		foreach(Collider2D c in PlayerShip.GetComponentsInChildren<Collider2D>()) {
			//c.enabled = false;
		}
		StopCoroutine(SpawnLevel());

		StartCoroutine(PlayerDeathRoutine());
	}

	IEnumerator PlayerDeathRoutine() {
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
		for(int i = 0; i < PointCutoffs.Count; i++) {
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

	IEnumerator SpawnLevel() {
		Meteor m1 = Instantiate(meteorPrefab, new Vector2(-7.5f, -0.5f), Quaternion.identity);
		m1.Init(Colors.Blue, 180, 3, 0.5f);

		yield return new WaitForSeconds(3f); //3

		Meteor m2 = Instantiate(meteorPrefab, new Vector2(-5f, -5f), Quaternion.identity);
		m2.Init(Colors.Blue, 225, 3, 0.5f);

		yield return new WaitForSeconds(3f); //6

		Meteor m3 = Instantiate(meteorPrefab, new Vector2(-5f, -5f), Quaternion.identity);
		m3.Init(Colors.Green, 225, 3, 0.5f);

		EMP e = Instantiate(EMPPrefab, new Vector2(1.5f, 0f), Quaternion.identity);
		e.Init(Colors.Yellow);

		yield return new WaitForSeconds(3f); //9

		Meteor m4 = Instantiate(meteorPrefab, new Vector2(-7.5f, 0), Quaternion.identity);
		m4.Init(Colors.Red, 180, 2, 0.5f);

		Meteor m5 = Instantiate(meteorPrefab, new Vector2(7.5f, 0f), Quaternion.identity);
		m5.Init(Colors.Blue, 0, 2, 0.5f);

		yield return new WaitForSeconds(5f); // 14

		LaserShip l1 = Instantiate(enemyShipPrefab);
		l1.Init(4, startRotation: -45, numberOfShots: 3, timeBtwShots: 4, color: Colors.Red);

		yield return new WaitForSeconds(2f); //16

		LaserShip l2 = Instantiate(enemyShipPrefab);
		l2.Init(4, startRotation: 135, numberOfShots: 3, timeBtwShots: 4, color: Colors.Red);

		yield return new WaitForSeconds(27f); //32

		OrbitingEnemy o1 = Instantiate(orbiterPrefab, new Vector2(5f, 2f), Quaternion.identity);
		o1.Init(Colors.Green, angle:0);

		yield return new WaitForSeconds(8f); //40

		OrbitingEnemy o2 = Instantiate(orbiterPrefab, new Vector2(5f, -2f), Quaternion.identity);
		o2.Init(Colors.Blue, angle:0);

		yield return new WaitForSeconds(3.5f); //45

		Meteor m6 = Instantiate(meteorPrefab, new Vector2(7.5f, 0f), Quaternion.identity);
		m6.Init(Colors.Blue, 0, 3, 0.5f);

		yield return new WaitForSeconds(3f); //48

		PointBeam p1 = Instantiate(pointBeamPrefab, new Vector2(-3,2), Quaternion.identity);
		p1.Init(Colors.Yellow, 20f);

		yield return new WaitForSeconds(2f); //50

		EMP e2 = Instantiate(EMPPrefab, new Vector2(-1f, 1f), Quaternion.identity);
		e2.Init(Colors.Yellow);

		yield return new WaitForSeconds(6f); //56

		LaserShip l3 = Instantiate(enemyShipPrefab);
		l3.Init(4, startRotation: 45, numberOfShots: 3, timeBtwShots: 3, color: Colors.Yellow);

		EMP e3 = Instantiate(EMPPrefab, new Vector2(1.5f, 0f), Quaternion.identity);
		e3.Init(Colors.Blue);

		yield return new WaitForSeconds(5f); //61

		Meteor m7 = Instantiate(meteorPrefab, new Vector2(5f, -4f), Quaternion.identity);
		m7.Init(Colors.Blue, -30, 5, 0.5f);

		yield return new WaitForSeconds(9f); //70

		Meteor m8 = Instantiate(meteorPrefab, new Vector2(7.5f, 0.5f), Quaternion.identity);
		m8.Init(Colors.Yellow, 0, 3, 0.5f);

		Meteor m9 = Instantiate(meteorPrefab, new Vector2(7.5f, -0.5f), Quaternion.identity);
		m9.Init(Colors.Green, 0, 3, 0.5f);

		yield return new WaitForSeconds(5f); //75

		Meteor m10 = Instantiate(meteorPrefab, new Vector2(0.5f, 7.5f), Quaternion.identity);
		m10.Init(Colors.Red, 90, 5, 0.5f);

		Meteor m11 = Instantiate(meteorPrefab, new Vector2(-0.5f, -7.5f), Quaternion.identity);
		m11.Init(Colors.Blue, -90, 5, 0.5f);

		yield return new WaitForSeconds(10f); //80

		OrbitingEnemy o3 = Instantiate(orbiterPrefab, new Vector2(-5f, 2f), Quaternion.identity);
		o3.Init(Colors.Red, angle: 180);

		yield return new WaitForSeconds(1f);

		OrbitingEnemy o4 = Instantiate(orbiterPrefab, new Vector2(-5f, -2f), Quaternion.identity);
		o4.Init(Colors.Green, angle: 180);

		yield return new WaitForSeconds(9f); //90

		BeginEndLevel();
	}

	public void ProcessInputs(InputPackage p) {
		PlayerShip.Rotate(p);
	}
}
