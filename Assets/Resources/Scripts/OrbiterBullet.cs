using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbiterBullet : Damager {

	public Vector3 Movement { get; set; }

	private ParticleSystem particleSystem;

	// Use this for initialization
	protected override void Start() {
		Color c = Utils.GetColorFromGameColor(GameColor);

		Color ahalf = new Color(c.r / 2f, c.g / 2f, c.b / 2f);

		particleSystem = GetComponentInChildren<ParticleSystem>();
		var main = particleSystem.main;
		//main.startColor = new ParticleSystem.MinMaxGradient( new Color(0.5f,0.5f,0.5f), new Color(0.5f, 0, 0) );
		//trailRenderer.startColor = trailRenderer.endColor = Color.white;//Utils.GetColorFromGameColor(GameColor);

		//StartCoroutine(Fire());
	}


	public void Init(Colors c, Vector3 direction) {
		GameColor = c;
		Movement = direction.normalized * 15f;

		transform.position -= direction.normalized * 0.1f;
	}

	public override void HitCore() {
		base.HitCore();
		GameManager.Instance.MainCameraController.Shake(-Movement/3.5f);
		Movement = Vector3.zero;
	}

	// Update is called once per frame
	protected void FixedUpdate() {
		transform.position += Movement * Time.fixedDeltaTime;
	}

	IEnumerator Fire() {
		float startTime = Time.time;
		float journeyTime = 0.75f;
		while (Time.time - startTime < journeyTime) {
			float jTime = (Time.time - startTime) / journeyTime;
			transform.localScale = new Vector3(Mathf.Lerp(0.1f, 0.75f, jTime), 1f, 1f);
			yield return new WaitForEndOfFrame();
		}
	}
}
