using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrailerLasershipBullet : Damager {
	public Vector3 Movement { get; set; }
	public TrailerLasership SpawningShip { get; set; }
	public bool Charging { get; set; }

	private SpriteRenderer spriteRenderer;
	private ParticleSystem dust, sparkle;

	public float ShieldAnimationTime;

	private Color color;
	private AudioSource audio;
	public AudioClip fireSound;

	// Use this for initialization
	protected override void Start() {
		color = SpawningShip.ShipColor;

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = color;
		spriteRenderer.material.SetFloat("_Cutoff", 0.1f);

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();

		sparkle = ps.First(p => p.main.loop);
		var em = sparkle.emission;
		em.enabled = false;

		var main = sparkle.main;
		main.startColor = new ParticleSystem.MinMaxGradient(Color.white, color);

		dust = ps.First(p => !p.main.loop);

		audio = GetComponent<AudioSource>();
		audio.volume /= 8f;
	}

	public void Init(TrailerLasership ship, float direction, float speed, int damage = 1) {
		GameColor = ship.GameColor;
		transform.localRotation = Quaternion.Euler(0, 0, direction);

		Damage = damage;

		SpawningShip = ship;

		Movement = Utils.AngleToVector(direction) * speed;
		Charging = true;
	}

	public override void HitShield() {
		base.HitShield();
		StartCoroutine(HitTarget(0.1f));
	}

	public void HitShield(float animationDuration) {
		base.HitShield();
		StartCoroutine(HitTarget(animationDuration));
	}

	public override void HitCore(bool screenshake = true) {
		base.HitCore();
		if (screenshake) {
			GameManager.Instance.MainCameraController.Shake(-Movement);
		}
		StartCoroutine(HitTarget(0.1f));
	}

	// Update is called once per frame
	protected override void FixedUpdate() {
		if (!Charging)
			transform.position += Movement * Time.fixedDeltaTime;
	}

	public void StartCharging() {
		StartCoroutine(Fire());
		audio.Play();
	}

	IEnumerator Fire() {
		//charge
		var em = sparkle.emission;
		em.enabled = true;

		float startTime = Time.time;
		float ttime = 3f;
		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			spriteRenderer.material.SetFloat("_Cutoff", Mathf.Lerp(0.1f, 0.55f, jTime));
			yield return new WaitForEndOfFrame();
		}

		Charging = false;
		GetComponent<Animator>().SetBool("Launched", true);
		//audio.clip = fireSound;
		//audio.Play();
		dust.Emit(20);
	}

	IEnumerator HitTarget(float duration) {
		float startTime = Time.time;
		GetComponent<CircleCollider2D>().enabled = false;
		Movement = Vector2.zero;
		while (Time.time - startTime < duration + Time.deltaTime) {
			spriteRenderer.material.SetFloat("_XAlpha", (Time.time - startTime) / duration);
			yield return new WaitForEndOfFrame();
		}

		//allow particles to disappear before destroying
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds(1f);

		Destroy(this.gameObject);
	}
}
