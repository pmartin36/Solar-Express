using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour {

	Core core;
	Shield[] shields;

	SpriteRenderer ShipTrail;

	public SpriteRenderer Cracks {
		get {
			return core.GetComponentsInChildren<SpriteRenderer>().First(s => s.gameObject != core.gameObject);
		}
	}

	public void Start() {
		(GameManager.Instance.ContextManager as LevelManager).PlayerShip = this;
		transform.localScale = Vector2.one * 0.01f;
		core = GetComponentInChildren<Core>();
		shields = GetComponentsInChildren<Shield>();

		ShipTrail = GameObject.FindGameObjectWithTag("ShipTrail").GetComponent<SpriteRenderer>();
		ShipTrail.transform.localScale = Vector2.one * 0.01f;
	}

	public void Rotate(InputPackage r) {
		Vector3 localRot = transform.localRotation.eulerAngles;
		transform.localRotation = Quaternion.Euler(localRot.x,localRot.y,r.AngleDiff);
	}

	public void ChangePlayerSize(Vector2 start, Vector2 end, float ttime = 4f) {
		StartCoroutine(ChangeSize(start, end));
	}

	private IEnumerator ChangeSize(Vector3 start, Vector3 end, float ttime = 4f) {
		//disable collision while changing size -- performance reasons
		foreach(Shield s in this.shields) {
			s.SetColliderActive(false);
		}

		float starttime = Time.time;

		List<Shield> shields = GetComponentsInChildren<Shield>().ToList();
		List<Vector3> shieldPositions = shields.Select( s => s.transform.localPosition ).ToList();

		Vector2 trailStart, trailEnd;
		Color trailStartAlpha, trailEndAlpha;
		if (end.sqrMagnitude > start.sqrMagnitude) {
			trailStart = Vector2.one * 0.01f;
			trailEnd = Vector2.one * 1.2f;

			trailStartAlpha = new Color(1,1,1,0.15f);
			trailEndAlpha = new Color(1,1,1,0.2f);
		}
		else {
			trailEnd = Vector2.one * 0.01f;
			trailStart = Vector2.one * 1.2f;

			trailEndAlpha = new Color(1, 1, 1, 0.15f);
			trailStartAlpha = new Color(1, 1, 1, 0.2f);
		}


		while( Time.time - starttime < ttime + Time.deltaTime) {
			float jtime = (Time.time - starttime) / ttime;
			transform.localScale = Vector2.Lerp(start, end, jtime);
			ShipTrail.transform.localScale = Vector3.Lerp( trailStart, trailEnd, jtime);

			ShipTrail.color = Color.Lerp( trailStartAlpha, trailEndAlpha, jtime);

			//fixing a bug where shield positions would round to 0 and stay there
			for (int i = 0; i < shields.Count; i++) {
				shields[i].transform.localPosition = shieldPositions[i];
			}

			yield return new WaitForEndOfFrame();
		}

		//re-enable collision
		foreach (Shield s in this.shields) {
			s.SetColliderActive(true);
		}
	}
}