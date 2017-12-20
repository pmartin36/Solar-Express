using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStar : MonoBehaviour {

	public Color OutsideColor {
		get {
			return Outside.color;
		}
		set {
			Outside.color = value;
		}
	}

	public Color InsideColor {
		get
		{
			return Inside.color;
		}
		set
		{
			Inside.color = value;
		}
	}

	private Image Outside;
	private Image Inside;

	public float Cutoff { get; set; }
	public bool Active { get; set; }

	ParticleSystem ps;

	public void Init(int cutoff) {
		Outside = GetComponent<Image>();
		OutsideColor = Color.clear;
		Inside = GetComponentsInChildren<Image>().First(g => g.gameObject != this.gameObject);
		InsideColor = Color.clear;

		Active = false;
		Cutoff = cutoff;

		ps = GetComponentInChildren<ParticleSystem>();
		ps.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void CheckIfActive(int score) {
		if(!Active && score >= Cutoff) {
			SetActive();
		}
	}

	void SetActive() {
		InsideColor = Color.white;
		Active = true;
		ps.gameObject.SetActive(true);
		var psem = ps.emission;
		//psem.enabled = true;
	}
}
