using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbiterBullet : Damager {

	public Vector3 Movement { get; set; }
	public Vector3 FinalPosition { get; set; }
	public bool WillBeBlocked { get; set; }

	private float amountToMove;
	private Vector3 startPosition;

	private ParticleSystem particleSystem;

	private AudioSource audio;
	

	// Use this for initialization
	protected override void Start() {
		Color c = Utils.GetColorFromGameColor(GameColor);

		Color ahalf = new Color(c.r / 2f, c.g / 2f, c.b / 2f);

		particleSystem = GetComponentInChildren<ParticleSystem>();
		var main = particleSystem.main;
		//main.startColor = new ParticleSystem.MinMaxGradient( new Color(0.5f,0.5f,0.5f), new Color(0.5f, 0, 0) );
		//trailRenderer.startColor = trailRenderer.endColor = Color.white;//Utils.GetColorFromGameColor(GameColor);

		//StartCoroutine(Fire());

		audio = GetComponent<AudioSource>();
		GameManager.Instance.ContextManager.AddAudioSource(audio);
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
	}


	public void Init(Colors c, Vector3 direction, Vector3 finalPosition, bool willBeBlocked = false) {
		GameColor = c;
		Movement = direction.normalized * 15f;

		transform.position -= direction.normalized * 0.1f;
		
		FinalPosition = finalPosition;
		WillBeBlocked = willBeBlocked;

		startPosition = transform.position;
		amountToMove = (startPosition - FinalPosition).magnitude;
	}

	public override void HitCore(bool screenshake = true) {
		base.HitCore();
		if(screenshake) {
			GameManager.Instance.MainCameraController.Shake(-Movement/3.5f);
		}
		Movement = Vector3.zero;
		StartCoroutine(DestroyAfterSeconds());
	}

	public override void HitShield() {
		base.HitShield();
		StartCoroutine(DestroyAfterSeconds());
	}

	public IEnumerator DestroyAfterSeconds() {
		GetComponent<CircleCollider2D>().enabled = false;
		yield return new WaitForSeconds(2f);
		GameManager.Instance.ContextManager.RemoveAudioSource(audio);
		Destroy(this.gameObject);
	}

	// Update is called once per frame
	protected override void FixedUpdate() {
		Vector3 newPos = transform.position + Movement * Time.fixedDeltaTime;
		if( (newPos - startPosition).magnitude > amountToMove ) {
			newPos = FinalPosition;
		}
		transform.position = newPos;
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
