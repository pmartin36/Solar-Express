using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public EnemyShip enemyShipPrefab;
	public Meteor meteorPrefab;

	// Use this for initialization
	void Start () {
		
		StartCoroutine(SpawnLevel());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator SpawnLevel() {
		EnemyShip es = Instantiate(enemyShipPrefab);
		es.Init(new Vector2(1, -1) * 2, startRotation: 135, numberOfShots: 5, rotationBetweenShots: 0, timeBtwShots: 2f, color: Colors.Green);

		yield return new WaitForSeconds(1f);

		EnemyShip es2 = Instantiate(enemyShipPrefab);
		es2.Init(new Vector2(1,0) * 2, startRotation: 180, numberOfShots: 5, rotationBetweenShots: 0, timeBtwShots: 2f, color: Colors.Blue);

		Meteor m = Instantiate(meteorPrefab, new Vector2(-1,1) * 5, Quaternion.identity);
		m.Init(Colors.Red, 90, 5, 0.5f);
	}
}
