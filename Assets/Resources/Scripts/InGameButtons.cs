using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameButtons : MonoBehaviour {

	float startTime;
	protected Image image;

	// Use this for initialization
	public virtual void Start () {
		image = GetComponent<Image>();
	}
	

	public virtual void onTouch() {

	}

	// Update is called once per frame
	void Update () {
		
	}
}
