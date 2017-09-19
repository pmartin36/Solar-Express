using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotateAround : MonoBehaviour {

	public Vector3 around;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.parent.position, around, 100 * Time.deltaTime);
	}
}
