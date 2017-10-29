using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbiterBulletRing : MonoBehaviour {

	float startTime;

	public float AnimationDuration { get; set; }

	private Color _startColor;
	private Color _endColor;
	public Color Color {
		get {
			return _startColor;
		}
		set {
			_startColor = value;
			Color c = _startColor;
			c.a = 0;
			_endColor = c;
		}
	}

	SpriteRenderer spriteRenderer;
	Animator anim;

	Vector3 startScale, endScale;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = _startColor;

		startScale = transform.localScale;
		endScale = new Vector2(0.6f, 2f);	
		
		anim = GetComponent<Animator>();
		anim.Play("orbitalring");	
	}
	
	// Update is called once per frame
	void Update () {
		//float dtime = (Time.time - startTime) / AnimationDuration;
		//transform.localScale = Vector3.Lerp(startScale, endScale, dtime);
		//spriteRenderer.color = Color.Lerp(_startColor, _endColor, dtime);


		
	}

	public void DestroyRing() {
		Destroy(this.gameObject);	
	}
}
