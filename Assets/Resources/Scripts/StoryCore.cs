using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StoryCore : MonoBehaviour {

	Image shields;
	Image shieldSpawnEffect;
	Image core;

	public bool ShieldsSpawned;

	public float LightRingDistance;
	public float LightRingRotation;

	// Use this for initialization
	void Start () {
		var images = GetComponentsInChildren<Image>();
		core = images.First( g => g.gameObject == this.gameObject);
		shields = images.First( g => g.tag == "Shield" );
		shieldSpawnEffect = images.First( g => g.tag == "Damager" );
		StartCoroutine(CoreRings());
	}
	
	// Update is called once per frame
	void Update () {
		core.material.SetFloat("_MaxLightRadius", LightRingDistance);
		core.material.SetFloat("_Rotation", LightRingRotation);
	}

	public void SpawnShields() {
		StartCoroutine(Shields());
	}

	public void Despawn() {
		StartCoroutine(DespawnAll());
	}

	IEnumerator TransitionImageSerial(Image[] images, Color start, Color end, float jTime) {
		float startTime = Time.time;
		while ((Time.time - startTime) < jTime + Time.deltaTime) {
			Color c = Color.Lerp(start, end, (Time.time - startTime) / jTime);
			for (int i = 0; i < images.Length; i++) {
				Image image = images[i];			
				image.color = c;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator CoreRings() {
		float startTime;
		float jTime = 2f;
		while(true) {
			startTime = Time.time;
			while ((Time.time - startTime) < jTime + Time.deltaTime) {
				float ttime = (Time.time - startTime) / jTime;
				LightRingDistance = Mathf.Lerp(0, 0.7f, ttime);
				LightRingRotation = Mathf.Lerp(0, 6f, 0.6f*ttime);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	IEnumerator Shields() {
		yield return TransitionImageSerial(new [] { shieldSpawnEffect }, Color.clear, Color.white, 0.25f);
		shields.color = Color.white;
		yield return TransitionImageSerial(new [] { shieldSpawnEffect }, Color.white, Color.clear, 1.5f);
		ShieldsSpawned = true;
	}

	IEnumerator DespawnAll() {
		yield return TransitionImageSerial(new[] { core, shields }, Color.white, Color.clear, 1f);
	}
}
