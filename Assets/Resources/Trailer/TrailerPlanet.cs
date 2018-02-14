using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerPlanet : MonoBehaviour {


	public List<Sprite> Planets;
	public List<Color> Outlines;

	[HideInInspector]
	public SpriteRenderer planet;
	public SpriteRenderer outline;

	public float TimeToComplete;
	private float startTime;
	public bool Started;

	public Vector2 destination;

	// Use this for initialization
	public void StartAnimation() {
		planet = GetComponent<SpriteRenderer>();
		startTime = Time.time;
		Started = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (destination.x < transform.position.x) {
			transform.position += 2 * Vector3.left * Time.deltaTime;
		}
		else {
			transform.position = destination;
		}

		if (!Started) return;

		float progress = (Time.time - startTime) / TimeToComplete;
		float length = Planets.Count - 1;

		float pIndexfloat = Mathf.Min(4, (progress * 6));
		int pIndex = (int)pIndexfloat;
		int pIndexNext = Mathf.Min(4, pIndex+1);

		float frac = pIndexfloat - Mathf.Floor(pIndexfloat);

		float pprogress = 0.75f * progress + 0.25f;
		planet.sprite = Planets[pIndex];
		Color c = Color.Lerp(Outlines[pIndex], Outlines[pIndexNext], frac);
		c.a = Mathf.Lerp(0, 0.5f, pprogress);
		outline.color = c;

		planet.material.SetFloat("_Cutoff", Mathf.Min(1f, pprogress));
		planet.material.SetTexture("_NextTex", Planets[pIndexNext].texture);
		planet.material.SetFloat("_Frac", frac);		
	}
}
