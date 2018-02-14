using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerNormal : LevelManager {

	public LaserShip enemyShipPrefab;
	public OrbitingEnemy orbiterPrefab;
	public EMP EMPPrefab;
	public PointBeam pointBeamPrefab;
	public Meteor meteorPrefab;

	private float LevelLength = 125f;
	private float CurrentLevelTime = 0f;

	protected override void Update() {
		base.Update();
		if (!PlayerDead) {
			CurrentLevelTime += Time.deltaTime;
			ProgressBar.UpdateProgressBar(CurrentLevelTime / LevelLength);
		}
	}

	public override void StartLevelSpawn() {
		base.StartLevelSpawn();
		StartCoroutine(SpawnLevel());
	}

	public override void PlayerDied() {
		PlayerDead = true;
		foreach (Collider2D c in PlayerShip.GetComponentsInChildren<Collider2D>()) {
			//c.enabled = false;
		}
		StopCoroutine(SpawnLevel());

		StartCoroutine(base.PlayerDeathRoutine());
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
		o1.Init(Colors.Green, angle: 0);

		yield return new WaitForSeconds(8f); //40

		OrbitingEnemy o2 = Instantiate(orbiterPrefab, new Vector2(5f, -2f), Quaternion.identity);
		o2.Init(Colors.Blue, angle: 0);

		yield return new WaitForSeconds(3.5f); //45

		Meteor m6 = Instantiate(meteorPrefab, new Vector2(7.5f, 0f), Quaternion.identity);
		m6.Init(Colors.Blue, 0, 3, 0.5f);

		yield return new WaitForSeconds(3f); //48

		PointBeam p1 = Instantiate(pointBeamPrefab, new Vector2(-3, 2), Quaternion.identity);
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
}
