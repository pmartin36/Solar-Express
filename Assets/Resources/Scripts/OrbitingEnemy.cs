using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OrbitingEnemy : MonoBehaviour {

	Transform ObjectToOrbit;

	SpriteRenderer gunSpriteRenderer;
	SpriteRenderer spriteRenderer;
	SpriteRenderer sight;

	public float deltaDistance = 0f;
	
	private float _chargeAmount;
	public float ChargeAmount {
		get {
			return _chargeAmount;
		}
		private set {
			_chargeAmount = value;
			gunSpriteRenderer.material.SetFloat("_Cutoff", _chargeAmount);

			Color bottom = color;
			//bottom.a = value/2f+ 0.5f;
			bottom.a = value;
			spriteRenderer.color = bottom;

			gunSpriteRenderer.material.SetColor("_DetailColor", bottom);
		}
	}

	Colors GameColor;
	Color color;

	Vector3 Movement;

	bool Orbiting = false;
	bool Fired = false;
	float amountRotated = 0f;
	float rotationalMovement;

	public static OrbiterBulletRing RingPrefab;
	public static OrbiterBullet BulletPrefab;
	public static SpriteRenderer SightPrefab;

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	public void Init(Colors c, float moveSpeed = 2f, float angle = 0) {
		RingPrefab = RingPrefab ?? Resources.Load<OrbiterBulletRing>("Prefabs/OrbiterBulletRing");
		BulletPrefab = BulletPrefab ?? Resources.Load<OrbiterBullet>("Prefabs/OrbiterBullet");
		SightPrefab = SightPrefab ?? Resources.Load<SpriteRenderer>("Prefabs/Sight");


		GameColor = c;
		color = Utils.GetColorFromGameColor(c);

		SpriteRenderer [] srs = GetComponentsInChildren<SpriteRenderer>();
		gunSpriteRenderer = srs.Single( s => s.tag == "Gun");
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

		ChargeAmount = 0f;
		gunSpriteRenderer.material.SetColor("_DetailColor", color);

		transform.localRotation = Quaternion.Euler(0, 0, angle);
		Movement = Utils.AngleToVector(angle) * moveSpeed;
	}

	// Update is called once per frame
	void Update () {
		ObjectToOrbit = ObjectToOrbit ?? GameManager.Instance.PlayerShip.transform;
		Vector2 direction = ObjectToOrbit.transform.position - this.transform.position;

		gunSpriteRenderer.transform.rotation = Quaternion.Euler(0,0, Utils.VectorToAngle(direction)-90);

		if (!Orbiting) {
			float dist = Vector2.Distance(transform.position, ObjectToOrbit.position);
			if (dist < 2.5f && amountRotated <= 1) {
				//begin orbiting
				Orbiting = true;
				rotationalMovement = 2 * Mathf.PI * dist * Movement.magnitude;

				RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction, 5f, 1 << LayerMask.NameToLayer("Shield"));
				sight = Instantiate(SightPrefab, hit.point, Quaternion.identity);
				sight.material.SetColor("_Color", color);

				StartCoroutine(Charge());
			}
			else {
				transform.position += Movement * Time.deltaTime;
			}
		}
		else {
			float rot = rotationalMovement * Time.deltaTime;
			amountRotated += rot;
			transform.RotateAround(ObjectToOrbit.position, Vector3.forward, rot);
			transform.Rotate(0,0,-rot);

			transform.position -= deltaDistance * (Vector3)direction * Time.deltaTime;
			
			if(!Fired) {
				RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, 0.1f, direction, 5f, 1 << LayerMask.NameToLayer("Shield"));

				if (hit.collider != null) {
					Core hitCore = hit.collider.GetComponent<Core>();
					Shield hitShield = hit.rigidbody.GetComponent<Shield>();
					if((hitCore != null || hitShield.GameColor != this.GameColor) && ChargeAmount >= 1f) {
						//Fire missile
						Fired = true;
						
						float angle = Vector3.Angle(Vector2.right, this.transform.position);
						if (Vector3.Cross(Vector2.right, (Vector2)this.transform.position).z < 0) {
							angle *= -1;
						}

						OrbiterBulletRing obr = Instantiate(RingPrefab, transform.position, Quaternion.Euler(0,0,angle));
						obr.Color = Color.clear;
						obr.AnimationDuration = 1.5f;
						obr.transform.position += (Vector3)direction * 0.15f;

						OrbiterBullet ob = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
						ob.Init(GameColor, direction);

						StartCoroutine(Fire());				
						Destroy(sight.gameObject);
					}
					else {
						sight.transform.position = hit.point;
					}
				}
			}

			if(amountRotated >= 180) {
				//Orbiting = false;
				//Movement.x *= -1;
				ChargeAmount = 0f;
				rotationalMovement = 0f;
			}
		}
	}

	IEnumerator Charge() {
		while(ChargeAmount < 1) {
			if(Fired || amountRotated >= 180) {
				yield break;
			}
			else {
				ChargeAmount += 0.1f;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator Fire() {
		ChargeAmount = 0f;
		yield return new WaitForEndOfFrame();
		anim.Play("pushback");
	}
}
