using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour {

	Core core;
	Shield[] shields;

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

		while( Time.time - starttime < ttime + Time.deltaTime) {
			transform.localScale = Vector2.Lerp(start, end, (Time.time - starttime) / ttime);

			//fixing a bug where shield positions would round to 0 and stay there
			for(int i = 0; i < shields.Count; i++) {
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