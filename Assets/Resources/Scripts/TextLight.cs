using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLight : MonoBehaviour {

	public static float ratio;

	// Use this for initialization
	void OnEnable () {
		if(ratio <= 0.0001f) {
			ratio = (Camera.main.aspect * 1.5f);
		}
		Light light = GetComponent<Light>();
		light.spotAngle *= ratio; 
		light.range *= ratio;
		//light.intensity /= ratio;
	}
	
}
