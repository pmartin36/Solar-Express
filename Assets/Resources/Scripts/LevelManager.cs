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

	public Ship PlayerShip;
	public bool MenuOpen { get; set; }
	public GameObject Menu;

	public PointManager PointManager;
	public ProgressBarManager ProgressBar;
	public Planet StartingPlanet, EndingPlanet;

	public ParticleSystem FingerParticles;

	private float LevelLength = 90f;
	private float CurrentLevelTime = 0f;

	//if CampaignMode, don't show pt sources
	public bool CampaignMode;
	public bool PlayerDead;

	public LevelFailMenu LevelFail;
	public LevelSuccessMenu LevelSuccess;

	public List<int> PointCutoffs;

	// Use this for initialization
	void Start () {
		MenuOpen = false;
		GameManager.Instance.RegisterContextManager(this);
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
		float ttime = 3f;
		float startTime = Time.time;
		while (Time.time - startTime < ttime) {
			float jTime = (Time.time - startTime) / ttime;
			cracks.material.SetFloat("_Cutoff", 0.6f * jTime);

			if(!LevelFail.gameObject.activeInHierarchy && jTime > 0.95f) {
				LevelFail.gameObject.SetActive(true);
			}

			yield return new WaitForEndOfFrame();
		}

		if(!LevelFail.gameObject.activeInHierarchy) {
			LevelFail.gameObject.SetActive(true);
		}
	}

	IEnumerator EndLevel() {
		EndingPlanet.Moving = true;
		yield return new WaitForSeconds(3f);

		float playerSizeChangeTime = 4f;
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
		Meteor m = Instantiate(meteorPrefab, new Vector2(0, 1.5f) * 5, Quaternion.identity);
		m.Init(Colors.Blue, 90, 5, 0.5f);
		Meteor m2 = Instantiate(meteorPrefab, new Vector2(0, -1.5f) * 5, Quaternion.identity);
		m2.Init(Colors.Red, -90, 5, 0.5f);
		Meteor m3 = Instantiate(meteorPrefab, new Vector2(1.5f, 0) * 5, Quaternion.identity);
		m3.Init(Colors.Green, 0, 5, 0.5f);
		Meteor m4 = Instantiate(meteorPrefab, new Vector2(-1.5f, 0) * 5, Quaternion.identity);
		m4.Init(Colors.Yellow, 180, 5, 0.5f);

		yield return new WaitForSeconds(1f);

		

		EMP e = Instantiate(EMPPrefab, new Vector2(1.5f, 0f), Quaternion.identity);
		e.Init(Colors.Green);
		EMP e2 = Instantiate(EMPPrefab, new Vector2(1.5f, 1f), Quaternion.identity);
		e2.Init(Colors.Blue);
		EMP e3 = Instantiate(EMPPrefab, new Vector2(-1.5f, -1f), Quaternion.identity);
		e3.Init(Colors.Red);

		OrbitingEnemy o = Instantiate(orbiterPrefab, new Vector2(-5f, -2f), Quaternion.identity);
		o.Init(Colors.Yellow);

		//OrbitingEnemy o2 = Instantiate(orbiterPrefab, new Vector2(0, -7f), Quaternion.identity);
		//o2.Init(Colors.Blue, angle: 90);

		yield return new WaitForSeconds(2f);

		LaserShip es = Instantiate(enemyShipPrefab);
		es.Init(4, startRotation: 135, numberOfShots: 2, rotationBetweenShots: 0, color: Colors.Red);

		yield return new WaitForSeconds(1f);

		//LaserShip es2 = Instantiate(enemyShipPrefab);
		//es2.Init(3, startRotation: 180, numberOfShots: 1, rotationBetweenShots: 0,  color: Colors.Green);

		//yield return new WaitForSeconds(1f);

		//LaserShip es3 = Instantiate(enemyShipPrefab);
		//es3.Init(3, startRotation: 90, numberOfShots: 2, rotationBetweenShots: 0, timeBtwShots: 4f, color: Colors.Blue);

		yield return new WaitForSeconds(15f);
		BeginEndLevel();
	}

	public void ProcessInputs(InputPackage p) {
		PlayerShip.Rotate(p);
	}
}
