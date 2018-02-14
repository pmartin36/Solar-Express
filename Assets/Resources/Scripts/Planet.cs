using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour {

	public bool Moving;

	public Vector2 Velocity;

	private float distanceToTravel;
	public Vector2 StartPosition, EndPosition;

	public bool IsStartingPlanet;
	private SpriteRenderer spriteRenderer, planetOutline;

	// Use this for initialization
	void Start () {
		StartPosition = transform.position;
		distanceToTravel = Vector2.Distance(StartPosition, EndPosition);

		spriteRenderer = GetComponent<SpriteRenderer>();
		planetOutline = GetComponentsInChildren<SpriteRenderer>().First( g => g.gameObject != this.gameObject );

		//if this is the starting planet
		if ( StartPosition.sqrMagnitude < 1 ) {
			IsStartingPlanet = true;
		}
		else {
			spriteRenderer.material.SetFloat("_Cutoff", 0);
		}

		
	}
	
	// Update is called once per frame
	void Update () {
		if(Moving) {
			transform.Translate(Velocity * Time.deltaTime);
		}

		float dist = Vector2.Distance(StartPosition, transform.position);
		LevelManager lm = (GameManager.Instance.ContextManager as LevelManager);
		if ( dist >= distanceToTravel ) {
			transform.position = EndPosition;
			
			if(IsStartingPlanet) {
				if(!lm.LevelStarted) {
					lm.StartLevelSpawn();
				}
				Destroy(this.gameObject);
			}
		}
		else if( dist > 0.8*distanceToTravel && IsStartingPlanet ) {
			if (!lm.LevelStarted) {
				lm.StartLevelSpawn();
			}
		}
	}

	public IEnumerator Colorize() {
		float ttime = 5f;
		float startTime = Time.time;
		while(Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			spriteRenderer.material.SetFloat("_Cutoff", jTime * 2f);

			var planetOutlineColor = planetOutline.color;
			planetOutlineColor.a = jTime * 0.5f;
			planetOutline.color = planetOutlineColor;

			yield return new WaitForEndOfFrame();
		}
	}
}
