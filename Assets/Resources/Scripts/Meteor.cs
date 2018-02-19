using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Meteor : Damager {

	private ParticleSystem particleSystem;
	private Vector3 Movement;

	public ParticleSystem collisionParticles;

	private AudioSource audio;
	private bool playedAudio = false;

	private Color color;
	private float size;
	private float startTime;
	private int points;

	// Use this for initialization
	protected override void Start () {
		startTime = Time.time;
	}

	public void Init(MeteorParameters p) {
		transform.position = new Vector2(p.x, p.y);
		Init(
			p.GameColor,
			p.Angle,
			p.Velocity,
			p.ParticleSize,
			p.Damage
		);

		points = p.Points;
		(GameManager.Instance.ContextManager as LevelManager).TotalAvailablePoints += points;
	}

	public void Init(Colors c, float angle = 0f, float vel = 5f, float particleSize = 0.5f, int damage = 1) {
		base.Init();
		GameColor = c;

		Damage = damage;

		angle -= 45;
		transform.localRotation = Quaternion.Euler(0,0,angle);
		Movement = Utils.AngleToVector(angle-135) * vel;

		particleSystem = GetComponent<ParticleSystem>();
		var main = particleSystem.main;
		var col = particleSystem.colorOverLifetime;

		size = particleSize;
		main.startSize = size;

		audio = GetComponent<AudioSource>();
		GameManager.Instance.ContextManager.AddAudioSource(audio);
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
		audio.loop = true;

		Color replacementColor;

		switch (c) {
			default:
			case Colors.Red:
				color = Color.red;
				replacementColor = new Color(0.5f, 0f, 0);
				break;
			case Colors.Green:
				color = Color.green;
				replacementColor = new Color(0f, 0.5f, 0);
				break;
			case Colors.Blue:
				color = Color.cyan;
				replacementColor = new Color(0f, 0, 1f);
				break;
			case Colors.Yellow:
				color = Color.yellow;
				replacementColor = new Color(0.5f, 0.5f, 0);
				break;					
		}

		main.startColor = new ParticleSystem.MinMaxGradient(Color.white, color);

		ParticleSystem.MinMaxGradient colorGradient = col.color;
		GradientColorKey[] keys = colorGradient.gradient.colorKeys;
		keys[1] = new GradientColorKey(replacementColor, 0.5f);
		colorGradient.gradient.colorKeys = keys;
		col.color = colorGradient;

		points = 2000;
	}

	protected override void Update() {
		base.Update();
		if( Time.time - startTime > 15f ) {
			Destroy();
		}

		float d = (transform.position.magnitude - 1.5f);
		float dsqr = (transform.position.sqrMagnitude - 2.5f);
		audio.volume = Mathf.Lerp(0.8f, 0f, d);
		audio.pitch = Mathf.Lerp(1, 0.5f, d);

		//if (!playedAudio) {
		//	float currentsqr = transform.position.sqrMagnitude;
		//	float newsqr = (transform.position + Movement * Time.fixedDeltaTime * 10f).sqrMagnitude;
		//	if (newsqr > currentsqr) {
		//		audio.Play();
		//		playedAudio = true;
		//	}
		//}		
	}

	// Update is called once per frame
	protected override void FixedUpdate () {
		transform.position += Movement * Time.fixedDeltaTime;
	}

	public override void HitShield() {
		(GameManager.Instance.ContextManager as LevelManager).PointManager.IncrementPoints(points, "Meteor Blocked", color);
		StartCoroutine(HitObject());
	}

	public override void HitCore(bool screenshake = true) {
		if (screenshake) {
			GameManager.Instance.MainCameraController.Shake(-Movement);
		}
		StartCoroutine(HitObject());	
	}

	private IEnumerator HitObject() {
		GetComponent<CircleCollider2D>().enabled = false;
		Movement = Vector3.zero;
		audio.Stop();

		var em = particleSystem.emission;
		em.enabled = false;

		ParticleSystem ps = Instantiate(collisionParticles, this.transform);
		var ps_main = ps.main;
		ps_main.startColor = new ParticleSystem.MinMaxGradient(Color.white, color);

		var ps_emitter = ps.emission;

		var burst = new ParticleSystem.Burst[1];
		ps_emitter.GetBursts(burst);
		burst[0].maxCount = burst[0].minCount = (short)(8 * (size/0.5f));

		ps_emitter.SetBursts(burst);

		yield return new WaitForSeconds(3f);
		Destroy();
	}

	private void Destroy() {
		GameManager.Instance.ContextManager.RemoveAudioSource(audio);
		Destroy(this.gameObject);
	}
}
