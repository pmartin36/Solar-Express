using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EMP : MonoBehaviour {

	private int TimeToDetonation;
	public Colors GameColor;

	private Color color;
	private Color secondaryColor;

	public SpriteRenderer spriteRenderer;
	public TMP_Text text;

	public static TMP_Text textPrefab;
	public static Canvas EMPTextCanvas;
	public static EMPExplosion explosionPrefab;

	private AudioSource audio;

	ParticleSystem ps;

	// Use this for initialization
	void Start () {
	}

	public void Init(Colors c, int detonationTime = 3) {
		GameColor = c;
		switch (c) {
			default:
			case Colors.Red:
				color = Color.red;
				secondaryColor = new Color(0.5f, 0f, 0);
				break;
			case Colors.Green:
				color = Color.green;
				secondaryColor = new Color(0f, 0.5f, 0);
				break;
			case Colors.Blue:
				color = Color.cyan;
				secondaryColor = new Color(0f, 0, 1f);
				break;
			case Colors.Yellow:
				color = Color.yellow;
				secondaryColor = new Color(0.5f, 0.5f, 0);
				break;
		}
		TimeToDetonation = detonationTime;

		textPrefab = textPrefab ?? Resources.Load<TMP_Text>("Prefabs/EMP Text");
		explosionPrefab = explosionPrefab ?? Resources.Load<EMPExplosion>("Prefabs/EMP Explosion");
		EMPTextCanvas = GameObject.FindGameObjectWithTag("EMPTextCanvas").GetComponent<Canvas>();

		text = Instantiate(textPrefab, EMPTextCanvas.transform);

		text.transform.localPosition = transform.position;
		text.rectTransform.position = Vector3.zero;

		text.text = TimeToDetonation.ToString();
		text.color = Color.clear;
		text.fontMaterial.SetColor("_UnderlayColor", color);
		text.fontMaterial.SetColor("_FaceColor", Color.black);


		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material.SetColor("_Color", color);
		spriteRenderer.material.SetColor("_SecondaryColor", secondaryColor);

		transform.localRotation = Quaternion.Euler(0,0,90);

		ps = GetComponentInChildren<ParticleSystem>();
		var main = ps.main;
		//main.startColor = Color.clear;

		StartCoroutine(Action());

		text.GetComponent<RectTransform>().position = transform.position + Vector3.one * 0.005f;

		audio = GetComponent<AudioSource>();
		GameManager.Instance.ContextManager.AddAudioSource(audio);
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
	}


	private void UpdateTimer(int c) {
		string newText = (TimeToDetonation - c).ToString();
		if(newText != text.text) {
			audio.Play();
		}
		text.text = newText;
	}

	private IEnumerator Action() {
		//spawn
		float startTime = Time.time;
		float spawnTime = 3f;
		var main = ps.main;
		while(Time.time - startTime < spawnTime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / spawnTime;
			Color lerpcolor = Color.Lerp(Color.clear, Color.white, jTime);

			//main.startColor = lerpcolor;
			text.color = lerpcolor;
			spriteRenderer.material.SetFloat("_Cutoff", Mathf.Lerp(0.75f, 0, jTime));

			yield return new WaitForEndOfFrame();
		}

		//play countdown on spawn
		audio.Play();

		//countdown
		int c = 0;
		while( c < TimeToDetonation ) {
			UpdateTimer(c);
			startTime = Time.time;
			while( Time.time - startTime < 1f + Time.deltaTime) {
				spriteRenderer.material.SetFloat("_Angle", Mathf.Lerp(c, c+1, Time.time - startTime) * 360 / TimeToDetonation);
				yield return new WaitForEndOfFrame();
			}
			c++;
		}
		
		//detonate
		EMPExplosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		explosion.Init(GameColor, color);

		GameManager.Instance.ContextManager.RemoveAudioSource(audio);

		Destroy(text.gameObject);
		Destroy(this.gameObject);
	}
}
