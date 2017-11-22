using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public void Start() {
		(GameManager.Instance.ContextManager as LevelManager).PlayerShip = this;	
	}

	public void Rotate(InputPackage r) {
		Vector3 localRot = transform.localRotation.eulerAngles;
		transform.localRotation = Quaternion.Euler(localRot.x,localRot.y,r.AngleDiff);
	}
}