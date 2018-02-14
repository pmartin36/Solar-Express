using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerMeteor : Damager {

	private ParticleSystem particleSystem;
	public float Movement;

	public ParticleSystem collisionParticles;

	private AudioSource audio;
	private bool playedAudio = false;

	private Color color;
	private float size;
	private float startTime;

	private bool _moving;
	public bool Moving {
		get {
			return _moving;
		}
		set {
			_moving = value;
			startTime = Time.time;
		}
	}

	// Use this for initialization
	protected override void Start() {
		particleSystem = GetComponent<ParticleSystem>();
		var main = particleSystem.main;
		var col = particleSystem.colorOverLifetime;

		//audio = GetComponent<AudioSource>();
		//audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
		//audio.loop = true;

		Color replacementColor;

		switch (GameColor) {
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
	}

	protected override void Update() {
		base.Update();
		if (Moving && Time.time - startTime > 30f) {
			Destroy();
		}
	}

	// Update is called once per frame
	protected override void FixedUpdate() {
		if(!Moving) return;
		transform.position += new Vector3(1,1,0) * -Movement * Time.fixedDeltaTime;
	}

	public override void HitShield() {
		StartCoroutine(HitObject());
	}

	private IEnumerator HitObject() {
		GetComponent<CircleCollider2D>().enabled = false;
		Movement = 0;
		//audio.Stop();

		var em = particleSystem.emission;
		em.enabled = false;

		ParticleSystem ps = Instantiate(collisionParticles, this.transform);
		var ps_main = ps.main;
		ps_main.startColor = new ParticleSystem.MinMaxGradient(Color.white, color);

		var ps_emitter = ps.emission;

		var burst = new ParticleSystem.Burst[1];
		ps_emitter.GetBursts(burst);
		burst[0].maxCount = burst[0].minCount = (short)(8 * (size / 0.5f));

		ps_emitter.SetBursts(burst);

		yield return new WaitForSeconds(3f);
		Destroy();
	}

	private void Destroy() {
		Destroy(this.gameObject);
	}
}
