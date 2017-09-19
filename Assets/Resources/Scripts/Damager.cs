using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour {

	public Colors GameColor;

	// Use this for initialization
	protected virtual void Start () {
		
	}

	// Update is called once per frame
	protected virtual void Update () {
		
	}

	public virtual void Init() {

	}

	public virtual void HitShield() {
		Debug.Log("Hit Shield");
	}

	public virtual void HitCore() {
		Debug.Log("Hit Core");
	}

	//protected virtual void OnCollisionEnter2D(Collision2D collision) {
	//	if(collision.otherCollider.tag == "Shield") {

	//	}
	//	else if(collision.otherCollider.tag == "Core") {

	//	}
	//}
}
