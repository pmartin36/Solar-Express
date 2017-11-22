using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Damager {

	private ParticleSystem particleSystem;
	private Vector3 Movement;

	public ParticleSystem collisionParticles;

	private Color color;
	private float size;

	// Use this for initialization
	protected override void Start () {
		
	}

	public void Init(Colors c, float angle = 0f, float vel = 5f, float particleSize = 0.5f) {
		base.Init();
		GameColor = c;

		transform.localRotation = Quaternion.Euler(0,0,angle);
		Movement = Utils.AngleToVector(angle-135) * vel;

		particleSystem = GetComponent<ParticleSystem>();
		var main = particleSystem.main;
		var col = particleSystem.colorOverLifetime;

		size = particleSize;
		main.startSize = size;

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
	}

	// Update is called once per frame
	protected override void Update () {
		transform.position += Movement * Time.deltaTime;
	}

	public override void HitShield() {
		(GameManager.Instance.ContextManager as LevelManager).PointManager.IncrementPoints(2000, "Meteor Blocked", color);
		StartCoroutine(HitObject());
	}

	public override void HitCore() {
		GameManager.Instance.MainCameraController.Shake(-Movement);
		StartCoroutine(HitObject());	
	}

	private IEnumerator HitObject() {
		GetComponent<CircleCollider2D>().enabled = false;
		Movement = Vector3.zero;

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
		Destroy(this.gameObject);
	}
}
