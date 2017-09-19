using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour {

	public float StartRotation { get; set; }
	public float TimeBetweenShots { get; set; }
	public int NumberOfShots { get; set; }
	public float RotationBetweenShots { get; set; }
	public Colors GameColor { get; set; }
	public Vector3 StartPosition { get; set; }

	public static Bullet bulletPrefab;

	private SpriteRenderer spriteRenderer;

	public void Init(Vector2 startPosition, float startRotation = 0, int numberOfShots = 1, float timeBtwShots = 1f, float rotationBetweenShots = 0, Colors color = Colors.Red) {
		StartRotation = startRotation;
		NumberOfShots = numberOfShots;
		RotationBetweenShots = rotationBetweenShots;
		GameColor = color;
		TimeBetweenShots = timeBtwShots;
		StartPosition = startPosition;
	}

	// Use this for initialization
	void Start () {
		transform.localRotation = Quaternion.Euler(0,0,StartRotation);
		spriteRenderer = GetComponent<SpriteRenderer>();

		spriteRenderer.color = Utils.GetColorFromGameColor(GameColor);

		bulletPrefab = bulletPrefab ?? Resources.Load<Bullet>("Prefabs/Bullet");

		StartCoroutine(PerformActions());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void Shoot() {
		Bullet b = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
		b.Init(this.GameColor, this.transform.position, transform.localRotation.eulerAngles.z, 4f);
	}

	IEnumerator PerformActions() {
		yield return Spawn();

		for(int i = 0; i < NumberOfShots; i++) {
			Shoot();			

			float startTime = Time.time;
			float startRotation = transform.localRotation.eulerAngles.z;
			float endRotation = startRotation + RotationBetweenShots;
			float scale = 2f;
			while( Time.time - startTime < TimeBetweenShots / scale + Time.deltaTime) {
				float jTime = (Time.time - startTime) / TimeBetweenShots;
				transform.localRotation = Quaternion.Euler( 0, 0, Mathf.Lerp(startRotation, endRotation, jTime * scale) );
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(TimeBetweenShots / scale);
		}
	}

	IEnumerator Spawn() {
		float startTime = Time.time;
		float journeyTime = 1f;

		float scale = transform.localScale.x;
		Vector3 startScale = new Vector3(5,1,1) * scale;
		Vector3 spawnPosition = StartPosition - Utils.AngleToVector(StartRotation).normalized * scale;

		while(Time.time - startTime < journeyTime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / journeyTime;
			transform.localScale = Vector3.Lerp(startScale, Vector3.one * scale, jTime);
			transform.position = Vector3.Lerp(spawnPosition, StartPosition, jTime);
			yield return new WaitForEndOfFrame();
		}
	}
}
