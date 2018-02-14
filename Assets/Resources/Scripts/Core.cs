using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Core : MonoBehaviour {

	public GameObject ShipHitPrefab;
	public int Health;

	public float LightRingDistance;
	public float LightRingRotation;

	public List<Texture> LightSprites;

	SpriteRenderer spriteRenderer;

	public event EventHandler CoreHit;

	// Use this for initialization
	void Start () {
		Health = 2;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		spriteRenderer.material.SetFloat("_MaxLightRadius", LightRingDistance);
		spriteRenderer.material.SetFloat("_Rotation", LightRingRotation);
		transform.Rotate(0,0, 20*Time.deltaTime);
	}

	public void OnTriggerEnter2D(Collider2D collision) {
		if (collision.tag == "Damager") {			
			Damager d = collision.GetComponent<Damager>();
			if(CoreHit != null) {
				CoreHit(this, null);
			}

			if(d.Damage == 0) {
				d.HitCore(true);
				StartCoroutine(ShipHit(collision.transform.position - this.transform.position, false));
				return;
			}

			Health--;
			StartCoroutine(ShipHit(collision.transform.position - this.transform.position));
			
			if(Health < 0) {
				//if the player got hit after their health reached 0

				//verify that the player is already in it's death animation...if not, start it
				if( !(GameManager.Instance.ContextManager as LevelManager).PlayerDead ) {
					(GameManager.Instance.ContextManager as LevelManager).PlayerDied();
				}

				//no screen shake on posthumous hits
				d.HitCore(false);
			}
			else {
				spriteRenderer.material.SetTexture("_ColorMap", LightSprites[Health]);
				if (Health <= 0) {
					(GameManager.Instance.ContextManager as LevelManager).PlayerDied();
					d.HitCore(true);
				}
				else {
					d.HitCore(true);
				}
			}
		}
	}

	IEnumerator ShipHit(Vector3 direction, bool shouldSmoke = true) {
		Vector3 position = transform.position + 0.75f * direction * transform.localScale.x;
		GameObject hit = Instantiate(ShipHitPrefab, position, Quaternion.Euler(0, 0, Utils.VectorToAngle(direction) + 90), this.transform);

		AudioSource hitAudio = hit.GetComponent<AudioSource>();
		GameManager.Instance.ContextManager.AddAudioSource(hitAudio);
		hitAudio.mute = !GameManager.Instance.PlayerInfo.SoundOn;

		var explosionps = hit.GetComponentsInChildren<ParticleSystem>();

		var explosion = explosionps.Single(p => !p.main.loop);
		var smoke = explosionps.Single(p => p.main.loop);
		smoke.gameObject.SetActive(shouldSmoke);
		smoke.gameObject.transform.position = position / 1.5f;
		
		yield return new WaitForSeconds(explosion.main.duration);

		
		if(shouldSmoke) {
			Destroy(explosion);
		}
		else {
			Destroy(hit.gameObject);
		}
		
	}
}
