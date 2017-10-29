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

	// Use this for initialization
	void Start () {
		Health = 4;
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
			d.HitCore();

			Health--;
			if(Health > 0) {
				StartCoroutine(ShipHit(collision.transform.position - this.transform.position));
				spriteRenderer.material.SetTexture("_ColorMap", LightSprites[Health]);
			}
			else {

			}
		}
	}

	IEnumerator ShipHit(Vector3 direction) {
		Vector3 position = transform.position + 0.75f * direction * transform.localScale.x;
		GameObject hit = Instantiate(ShipHitPrefab, position, Quaternion.Euler(0,0,Utils.VectorToAngle(direction)+90), this.transform);

		var explosionps = hit.GetComponentsInChildren<ParticleSystem>();

		var explosion = explosionps.Single(p => !p.main.loop);
		var smoke = explosionps.Single(p => p.main.loop);
		smoke.gameObject.transform.position = position / 1.5f;

		yield return new WaitForSeconds(explosion.main.duration);

		Destroy(explosion);
	}
}
