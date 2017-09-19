using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Damager {

	public Vector3 Movement { get; set; }

	private SpriteRenderer spriteRenderer;

	public float ShieldAnimationTime;

	// Use this for initialization
	protected override void Start () {
		Color c = Utils.GetColorFromGameColor(GameColor);

		Color ahalf = new Color(c.r/2f,c.g/2f,c.b/2f);
		
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.material.SetColor("_Color", ahalf);

		StartCoroutine(Fire());
	}
	
	public void Init(Colors c, Vector3 position, float direction, float speed) {
		GameColor = c;
		transform.localRotation = Quaternion.Euler(0,0,direction);

		Movement = Utils.AngleToVector(direction) * speed;
	}

	public override void HitShield() {
		base.HitShield();
		StartCoroutine(HitTarget(0.1f));
	}

	public void HitShield(float animationDuration) {
		base.HitShield();
		StartCoroutine(HitTarget(animationDuration));
	}

	public override void HitCore() {
		base.HitCore();

		GameManager.Instance.MainCameraController.Shake(-Movement);
		StartCoroutine(HitTarget(0.1f));
	}

	// Update is called once per frame
	protected override void Update () {
		transform.position += Movement * Time.deltaTime;
	}

	IEnumerator Fire() {
		float startTime = Time.time;
		float journeyTime = 0.75f;
		while(Time.time - startTime < journeyTime) {
			float jTime = (Time.time - startTime) / journeyTime;
			transform.localScale = new Vector3( Mathf.Lerp(0.1f, 0.75f, jTime), 1f, 1f);
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator HitTarget(float duration) {
		float startTime = Time.time;
		GetComponent<PolygonCollider2D>().enabled = false;
		while(Time.time - startTime < duration + Time.deltaTime) {
			spriteRenderer.material.SetFloat("_XAlpha", (Time.time - startTime) / duration);
			yield return new WaitForEndOfFrame();
		}
		Destroy(this.gameObject);
	}
}
