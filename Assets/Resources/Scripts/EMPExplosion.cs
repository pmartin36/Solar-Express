using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPExplosion : MonoBehaviour {

	public Colors GameColor;
	public float Radius, AlphaModifier;

	private Color primaryColor, secondaryColor;

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer centerSpriteRenderer;
	private CircleCollider2D circleCollider;

	private Transform center;
	//public Sprite[] sprites;

	public bool TestObject;

	// Use this for initialization
	void Start () {
		if(TestObject) {
			Init(GameColor, new Color(0.5f,0.0f,0));
		}
	}

	public void Init(Colors c, Color secondary) {
		GameColor = c;

		primaryColor = Utils.GetColorFromGameColor(c);

		secondaryColor = secondary;

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = primaryColor;
		spriteRenderer.material.SetColor("_Color", primaryColor);
		spriteRenderer.material.SetColor("_SecondaryColor", secondary);
		spriteRenderer.material.SetVector("_Center", this.transform.position);

		foreach (Transform t in transform) {
			center = t;
		}
		centerSpriteRenderer = center.GetComponent<SpriteRenderer>();

		Color darkgray = new Color(0.18f, 0.18f, 0.18f, 1);
		centerSpriteRenderer.material.SetColor("_TopLayerPrimaryColor", primaryColor);
		centerSpriteRenderer.material.SetColor("_TopLayerSecondaryColor", darkgray);
		centerSpriteRenderer.material.SetColor("_BotLayerPrimaryColor", secondary);
		centerSpriteRenderer.material.SetColor("_BotLayerSecondaryColor", darkgray);

		circleCollider = GetComponent<CircleCollider2D>();
		circleCollider.radius = 0f;

		UpdateExplosion();
		StartCoroutine(Action());
	}
	
	// Update is called once per frame
	void Update () {
		UpdateExplosion();
	}

	private void UpdateExplosion() {
		circleCollider.radius = Radius * 0.5f;
		spriteRenderer.material.SetFloat("_Radius", Radius);
		center.localScale = Vector3.one * Mathf.Min(0.3f, Radius);
		center.Rotate(0,0,1f);

		spriteRenderer.material.SetColor("_SecondaryColor", Color.Lerp(primaryColor, Color.white, Radius));
		spriteRenderer.material.SetFloat("_AlphaModifier", AlphaModifier);
		centerSpriteRenderer.material.SetColor("_BotLayerSecondaryColor", Color.Lerp(Color.black, Color.gray, Radius));
		centerSpriteRenderer.material.SetFloat("_AlphaModifier", AlphaModifier + 0.5f);
	}

	IEnumerator Action(){
		yield return new WaitForSeconds(1.1f);

		/*
		float jTime = 4f / sprites.Length;
		int transitioningToIndex = 0;

		while (transitioningToIndex < sprites.Length) {
			centerSpriteRenderer.material.SetFloat("_TexTransition", 0);
			var newsprite = sprites[transitioningToIndex];
			var newtexture = new Texture2D( (int)newsprite.textureRect.width, (int)newsprite.textureRect.height );
			Debug.Log(newsprite.textureRect);
			newtexture.SetPixels(
				newsprite.texture.GetPixels(
					(int)newsprite.textureRect.x,
					(int)newsprite.textureRect.y,
					(int)newsprite.textureRect.width,
					(int)newsprite.textureRect.height
				)
			);
			newtexture.Apply();
			centerSpriteRenderer.material.SetTexture("_TargetTex", newtexture);
			float startTime = Time.time;
			while (Time.time - startTime < jTime + Time.deltaTime) {
				float transition = (Time.time - startTime) / jTime;
				centerSpriteRenderer.material.SetFloat("_TexTransition", transition );
				yield return new WaitForEndOfFrame();
			}						
			centerSpriteRenderer.sprite = sprites[transitioningToIndex];
			transitioningToIndex++;		
		}
			
		yield return new WaitForSeconds(jTime);
		*/

		Destroy(this.gameObject);
	}

}
