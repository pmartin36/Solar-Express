﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserShip : MonoBehaviour {

	public float StartRotation { get; set; }
	public Vector3 Direction { get; set; }
	public float TimeBetweenShots { get; set; }
	public int NumberOfShots { get; set; }
	public float RotationBetweenShots { get; set; }
	public Colors GameColor { get; set; }
	public Vector3 StartPosition { get; set; }
	
	public static Bullet bulletPrefab;

	private SpriteRenderer spriteRenderer;
	private Color shipColor;

	private ParticleSystem EnginePS;
	private ParticleSystem [] FeedPS;

	public void Init(float endDistFromShip, float startRotation = 0, int numberOfShots = 1, float timeBtwShots = 1f, float rotationBetweenShots = 0, Colors color = Colors.Red) {
		StartRotation = startRotation;
		NumberOfShots = numberOfShots;
		RotationBetweenShots = rotationBetweenShots;
		TimeBetweenShots = timeBtwShots;
		
		Direction = Utils.AngleToVector(startRotation).normalized;
		StartPosition = -endDistFromShip * Direction;

		GameColor = color;
		shipColor = Utils.GetColorFromGameColor(color);
	}

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material.SetColor("_DetailColor", shipColor);
		spriteRenderer.material.SetFloat("_Cutoff", 0.99f);

		bulletPrefab = bulletPrefab ?? Resources.Load<Bullet>("Prefabs/Bullet");

		var pss = GetComponentsInChildren<ParticleSystem>();
		EnginePS = pss.Single(g => g.tag == "Laser Ship Engine");
		FeedPS = pss.Where(g => g.tag != "Laser Ship Engine").ToArray();

		foreach (ParticleSystem ps in FeedPS) {
			var main = ps.main;
			main.startColor = new ParticleSystem.MinMaxGradient(Color.white, shipColor);
		}

		StartCoroutine(PerformActions());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private Bullet SpawnBullet(float chargeTime) {
		Vector3 direction = Utils.AngleToVector(StartRotation).normalized;
		Bullet b = Instantiate(bulletPrefab, this.transform.position + 0.25f * direction, Quaternion.identity);
		b.Init(this, transform.localRotation.eulerAngles.z, 4f);
		return b;
	}

	IEnumerator PerformActions() {
		yield return Spawn();

		for(int i = 0; i < NumberOfShots; i++) {
			float scale = 2f;
			float startTime = Time.time;
			float chargeTime = 3f;

			Bullet b = SpawnBullet(chargeTime);
			Vector3 endPosition = transform.position + Direction / 2.5f;

			//move bullet out
			float moveOutTime = TimeBetweenShots * 0.7f;
			while (Time.time - startTime < moveOutTime + Time.deltaTime) {
				float jTime = (Time.time - startTime) / moveOutTime;
				b.transform.position = Vector3.Lerp(transform.position, endPosition, jTime);
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(TimeBetweenShots * 0.3f);

			b.StartCharging();

			//start charging
			foreach (ParticleSystem ps in FeedPS) {
				var em = ps.emission;
				em.enabled = true;
			}

			startTime = Time.time;
						
			yield return new WaitForSeconds(chargeTime * 0.7f);

			//stop charging particles
			foreach (ParticleSystem ps in FeedPS) {
				var em = ps.emission;
				em.enabled = false;
			}

			yield return new WaitUntil(() => !b.Charging);

			//discharge
			/*
			startTime = Time.time;
			while (Time.time - startTime < 0.5f + Time.deltaTime) {
				float jTime = (Time.time - startTime) / 0.1f;
				spriteRenderer.material.SetFloat("_Cutoff", Mathf.Lerp(0.01f, 0.99f, jTime));
				yield return new WaitForEndOfFrame();
			}			

			startTime = Time.time;
			float startRotation = transform.localRotation.eulerAngles.z;
			float endRotation = startRotation + RotationBetweenShots;
			while( Time.time - startTime < TimeBetweenShots / scale + Time.deltaTime) {
				float jTime = (Time.time - startTime) / TimeBetweenShots;
				transform.localRotation = Quaternion.Euler( 0, 0, Mathf.Lerp(startRotation, endRotation, jTime * scale) );
				yield return new WaitForEndOfFrame();
			}
			*/
		}
	}

	IEnumerator Spawn() {
		float startTime = Time.time;

		Vector3 spawnPosition = StartPosition - Utils.AngleToVector(StartRotation).normalized * 5;
		float sqrmag = (StartPosition - spawnPosition).sqrMagnitude;
		bool startSpin = false;
		float moveSpeed = 3;

		transform.localRotation = Quaternion.Euler(0, 0, StartRotation+180);
		transform.position = spawnPosition;

		
		while ((transform.position - spawnPosition).sqrMagnitude < sqrmag) {
			transform.position += Direction * moveSpeed * Time.deltaTime;

			float posdiff = (StartPosition - transform.position).sqrMagnitude;
			if ( posdiff < 1f ) {
				moveSpeed = Mathf.Max(0.5f, posdiff*2f);
				if(!startSpin) {
					startSpin = true;
					StartCoroutine(SpinMovement());
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SpinMovement() {
		float startTime = Time.time;
		float journeyTime = 1f;

		var em = EnginePS.emission;
		em.enabled = false;

		float startRotation = StartRotation + 180;;
		float endRotation = StartRotation;
		Debug.Log(string.Format("{0}, {1}", startRotation, endRotation));

		while (Time.time - startTime < journeyTime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / journeyTime;
			transform.localRotation = Quaternion.Euler(0,0, Mathf.Lerp(startRotation, endRotation, jTime));
			yield return new WaitForEndOfFrame();
		}
	}
}
