using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ContextManager {

	public LaserShip enemyShipPrefab;
	public Meteor meteorPrefab;
	public OrbitingEnemy orbiterPrefab;
	public EMP EMPPrefab;

	public Ship PlayerShip;
	public bool MenuOpen { get; set; }
	public GameObject Menu;

	public PointManager PointManager;

	// Use this for initialization
	void Start () {
		MenuOpen = false;
		GameManager.Instance.RegisterContextManager(this);

		SetMenuState();
		StartCoroutine(SpawnLevel());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleMenu () {
		MenuOpen = !MenuOpen;
		SetMenuState();
	}

	private void SetMenuState() {
		Menu.SetActive(MenuOpen);
		Time.timeScale = MenuOpen ? 0.00001f : 1f;
	}

	IEnumerator SpawnLevel() {
		Meteor m = Instantiate(meteorPrefab, new Vector2(-1, 1) * 5, Quaternion.identity);
		m.Init(Colors.Blue, 90, 5, 0.5f);

		yield return new WaitForSeconds(1f);

		EMP e = Instantiate(EMPPrefab, new Vector2(1.5f, 0f), Quaternion.identity);
		e.Init(Colors.Green);

		OrbitingEnemy o = Instantiate(orbiterPrefab, new Vector2(-5f, -2f), Quaternion.identity);
		o.Init(Colors.Yellow);

		//OrbitingEnemy o2 = Instantiate(orbiterPrefab, new Vector2(0, -7f), Quaternion.identity);
		//o2.Init(Colors.Blue, angle: 90);

		yield return new WaitForSeconds(2f);

		LaserShip es = Instantiate(enemyShipPrefab);
		es.Init(4, startRotation: 135, numberOfShots: 10, rotationBetweenShots: 0, timeBtwShots: 4f, color: Colors.Red);

		yield return new WaitForSeconds(1f);

		LaserShip es2 = Instantiate(enemyShipPrefab);
		es2.Init(3, startRotation: 180, numberOfShots: 5, rotationBetweenShots: 0, timeBtwShots: 2f, color: Colors.Green);

		//yield return new WaitForSeconds(1f);

		//LaserShip es3 = Instantiate(enemyShipPrefab);
		//es3.Init(3, startRotation: 90, numberOfShots: 2, rotationBetweenShots: 0, timeBtwShots: 4f, color: Colors.Blue);

	}

	public void ProcessInputs(InputPackage p) {
		PlayerShip.Rotate(p);
	}
}
