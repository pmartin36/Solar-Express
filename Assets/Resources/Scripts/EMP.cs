using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EMP : MonoBehaviour {

	private int TimeToDetonation;
	public Colors GameColor;

	Vector2 conversion;

	private Color color;
	private Color secondaryColor;

	public SpriteRenderer spriteRenderer;
	public TMP_Text text;

	public static TMP_Text textPrefab;
	public static Canvas EMPTextCanvas;
	public static EMPExplosion explosionPrefab;

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
		EMPTextCanvas = EMPTextCanvas ?? GameObject.FindGameObjectWithTag("EMPTextCanvas").GetComponent<Canvas>();

		text = Instantiate(textPrefab, EMPTextCanvas.transform);

		text.transform.localPosition = transform.position;
		text.rectTransform.position = Vector3.zero;

		text.text = TimeToDetonation.ToString();
		text.color = color;
		text.fontMaterial.SetColor("_UnderlayColor", color);
		text.fontMaterial.SetColor("_FaceColor", Color.black);

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.material.SetColor("_Color", color);
		spriteRenderer.material.SetColor("_SecondaryColor", secondaryColor);

		transform.localRotation = Quaternion.Euler(0,0,90);

		StartCoroutine(Action());

		Vector2 camSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize);
		Vector2 canvasSize = new Vector2(136/2f, 205/2f);
		conversion = new Vector2(canvasSize.x/camSize.x, canvasSize.y/camSize.y);
	}

	// Update is called once per frame
	void Update () {
		text.transform.localPosition = new Vector2((transform.localPosition.x+0.005f) * conversion.x, (transform.localPosition.y + 0.005f) * conversion.y);
	}

	private void UpdateTimer(int c) {
		text.text = (TimeToDetonation - c).ToString();
	}

	private IEnumerator Action() {
		//spawn

		//countdown
		int c = 0;
		while( c < TimeToDetonation ) {
			UpdateTimer(c);
			float startTime = Time.time;
			while( Time.time - startTime < 1f + Time.deltaTime) {
				spriteRenderer.material.SetFloat("_Angle", Mathf.Lerp(c, c+1, Time.time - startTime) * 360 / TimeToDetonation);
				yield return new WaitForEndOfFrame();
			}
			c++;
		}
		
		//detonate
		EMPExplosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		explosion.Init(GameColor, color);

		Destroy(text.gameObject);
		Destroy(this.gameObject);
	}
}
