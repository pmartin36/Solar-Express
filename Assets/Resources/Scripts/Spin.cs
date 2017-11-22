using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour {

	ParticleSystem particleSystem;

	// Use this for initialization
	void Start () {
		particleSystem = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3( -3 * Time.deltaTime, 0, 0);
	}
}
