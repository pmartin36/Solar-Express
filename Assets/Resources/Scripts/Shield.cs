using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

	public Colors GameColor;
	private int ringIndex = 0;

	private bool Disabled;

	List<Color> ringColor;
	List<Vector4> ringInfo;
	List<Coroutine> ringCoroutines;

	SpriteRenderer spriteRenderer;
	PolygonCollider2D polycollider;

	private static float lastShieldHitTime = 0;
	private static float lastShieldHitPitch = 0.95f;
	private AudioSource audio;

	public event EventHandler ShieldHit; 

	// Use this for initialization
	void Start () {
		ringColor = new List<Color>() { Color.white, Color.white };
		ringInfo = new List<Vector4>() { Vector4.zero, Vector4.zero };
		ringCoroutines = new List<Coroutine>(2);

		spriteRenderer = GetComponent<SpriteRenderer>();
		polycollider = GetComponent<PolygonCollider2D>();

		audio = GetComponent<AudioSource>();
		GameManager.Instance.ContextManager.AddAudioSource(audio);
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
	}
	
	// Update is called once per frame
	void Update () {
		spriteRenderer.material.SetInt("_NumRings", ringCoroutines.Count);
		spriteRenderer.material.SetColorArray("_RingColor", ringColor.ToArray());
		spriteRenderer.material.SetVectorArray("_RingInfo", ringInfo.ToArray());
	}

	public void OnTriggerEnter2D(Collider2D collision) {
		if(collision.tag == "Damager") {
			Damager d = collision.GetComponent<Damager>();
			if(d.GameColor == this.GameColor || d is OrbiterBullet) {
				if(ShieldHit != null) {
					ShieldHit(this, null);
				}
				float animationDuration = 0.25f;

				if (d is Bullet) {
					Bullet b = d as Bullet;
					animationDuration = 0.25f;
					b.HitShield(animationDuration/2.5f);
					b.HitShield();				
				}
				else if(d is Meteor) {
					Meteor m = d as Meteor;
					animationDuration = 0.25f;
					m.HitShield();
				}
				else if(d is OrbiterBullet) {
					OrbiterBullet m = d as OrbiterBullet;
					if(!m.WillBeBlocked) return;				
					d.HitShield();
				}
				d.GetComponent<Collider2D>().enabled = false;	

				Vector2 collisionCenter = (collision.transform.position - this.transform.position);
				ringInfo[ringIndex] = collisionCenter.normalized * 0.85f;

				if (ringCoroutines.Count < 2) {					
					ringCoroutines.Add(StartCoroutine(HitShield(Utils.GetColorFromGameColor(d.GameColor), ringIndex, animationDuration)));
				}
				else {
					StopCoroutine(ringCoroutines[ringIndex]);
					ringCoroutines[ringIndex] = StartCoroutine(HitShield(Utils.GetColorFromGameColor(d.GameColor), ringIndex, animationDuration));
				}

				ringIndex = (ringIndex + 1) % 2;
			}
			else {
				//projectile passes through
				//should apply a colored burn effect at that location

			}
		}
		else if(collision.tag == "EMP Explosion") {
			EMPExplosion s = collision.GetComponent<EMPExplosion>();
			if (s.GameColor == GameColor) {
				//disable shield for time
				DisableShieldForTime(4.5f);
			}
			else {
				
			}
		}
	}

	public void DisableShieldForTime(float time) {
		StartCoroutine(DisableForTime(time));
	}

	private void ToggleDisabled(bool disabled) {
		Disabled = disabled;

		Color c = spriteRenderer.color;
		c.a = disabled ? 0.3f : 1f;
		spriteRenderer.color = c;

		polycollider.enabled = !disabled;
	}

	IEnumerator DisableForTime(float time) {
		ToggleDisabled(true);

		yield return new WaitForSeconds(time);

		float startTime = Time.time;
		float jTime = 0.5f;
		while( Time.time - startTime < jTime + Time.deltaTime ) {
			Color c = spriteRenderer.color;
			c.a = Mathf.Lerp(0.3f, 1, (Time.time - startTime) / jTime);
			spriteRenderer.color = c;
			yield return new WaitForEndOfFrame();
		}

		Disabled = false;
		polycollider.enabled = true;
	}

	public void SetColliderActive(bool active) {
		polycollider.enabled = active;
	}

	IEnumerator HitShield(Color c, int index, float animationDuration) {
		if(Time.time - lastShieldHitTime > 5f) {
			lastShieldHitPitch = 0.9f;
		}
		else {
			lastShieldHitPitch = Mathf.Min(1.3f, lastShieldHitPitch + 0.05f);
		}
		lastShieldHitTime = Time.time;
		audio.pitch = lastShieldHitPitch;
		audio.Play();
		
		ringColor[index] = Color.white;
		Color halfColor = new Color(0.5f,0.5f,0.5f,0.5f);
		float startTime = Time.time;
		float halfduration = animationDuration/2f;
		while(Time.time - startTime < halfduration + Time.deltaTime) {
			float ttime = (Time.time - startTime) / animationDuration;
			Vector4 ring = ringInfo[index];
			ring.w = Mathf.Lerp(0, 2, ttime);
			ringInfo[index] = ring;
			yield return new WaitForEndOfFrame();
		}

		startTime = Time.time;
		animationDuration *= 1.5f;
		while (Time.time - startTime < animationDuration + Time.deltaTime) {
			Vector4 ring = ringInfo[index];
			float ttime = (Time.time - startTime) / animationDuration;
			ring.z = Mathf.Lerp(0, 2, ttime);
			ringInfo[index] = ring;
			ringColor[index] = Color.Lerp(Color.white, halfColor, ttime);
			yield return new WaitForEndOfFrame();
		}

		ringColor[index] = Color.clear;
		ringInfo[index] = Vector4.zero;
		//if (index < ringCoroutines.Count)
		//	ringCoroutines.RemoveAt(index);
	}
}
