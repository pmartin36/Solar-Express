using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TrailerManager : MonoBehaviour {

	public TMP_Text helper;
	public GameObject finalScreen;
	public TrailerPlanet planet;
	public GameObject player;
	public GameObject playertrail;

	private static RenderTexture Blurred;

	public Material blurMaterial;

	public int blurIterations = 5;
	public float blurLength = 1.5f;

	bool blurring = false;

	public GameObject meteorParent;

	Camera c;

	void Start() {
		Blurred = new RenderTexture(Screen.width >> 1, Screen.height >> 1, 0);
		c = GetComponent<Camera>();
		blurMaterial.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * blurLength, Blurred.texelSize.y * blurLength));

		StartCoroutine(Trailer());
	}


	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		if(!blurring) {
			Graphics.Blit(src, dst);
			return;
		}

		Graphics.SetRenderTarget(Blurred);
		GL.Clear(false, true, Color.clear);

		Graphics.Blit(src, Blurred);

		for (int i = 0; i < blurIterations; i++) {
			var temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			Graphics.Blit(Blurred, temp, blurMaterial, 0);
			Graphics.Blit(temp, Blurred, blurMaterial, 1);
			RenderTexture.ReleaseTemporary(temp);
		}

		Graphics.Blit(Blurred, dst);
	}

	IEnumerator HideHelperText(float ttime = 1f) {
		float startTime = Time.time;

		int startingBlurIterations = blurIterations;

		while(Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			blurIterations = (int)Mathf.Lerp(startingBlurIterations, 0, jTime);
			helper.color = Color.Lerp(Color.white, Color.clear, jTime);
			yield return new WaitForEndOfFrame();
		}

		blurring = false;
	}

	IEnumerator ShowHelperText(float ttime = 1f) {
		float startTime = Time.time;

		int startingBlurIterations = blurIterations;
		blurring = true;

		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			blurIterations = (int)Mathf.Lerp(0, 5, jTime);
			helper.color = Color.Lerp(Color.clear, Color.white, jTime);
			yield return new WaitForEndOfFrame();
		}	
	}

	IEnumerator ChangePlayerSize(Vector3 playerend, Vector3 trailend, float ttime) {
		float startTime = Time.time;
		Vector3 playerstart = player.transform.localScale;
		Vector3 trailstart = playertrail.transform.localScale;
		while( Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			player.transform.localScale = Vector3.Lerp(playerstart, playerend, jTime);
			playertrail.transform.localScale = Vector3.Lerp(trailstart, trailend, jTime);
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Trailer () {
		//start colorful meteor shower
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(ShowHelperText());	
		StartMeteors();
		ShieldCollidersActive(true);
		yield return new WaitForSeconds(1f);	
		yield return StartCoroutine(HideHelperText());	
		yield return new WaitForSeconds(2f);

		helper.text = "Color the \nUniverse";		
		yield return StartCoroutine(ShowHelperText());
		//hide play and load up end planet
		planet.gameObject.SetActive(true);
		planet.destination = Vector3.zero;
		StartCoroutine(ChangePlayerSize(Vector3.zero, Vector3.zero, 6f));
		yield return new WaitForSeconds(4f);
		yield return StartCoroutine(HideHelperText());
		yield return new WaitForSeconds(2f);
		//enable end planet and play animation
		planet.StartAnimation();

		yield return new WaitForSeconds(4f);

		helper.text = "Repel colorful threats";
		StartCoroutine(ChangePlayerSize(Vector3.one * 0.75f, Vector3.one * 1.2f, 4f));
		planet.destination = Vector2.left * 15f;
		yield return StartCoroutine(ShowHelperText());
		//hide planet and show player

		StartCoroutine(StartShips());
		yield return new WaitForSeconds(2f);
		ShieldCollidersActive(true);
		yield return StartCoroutine(HideHelperText());

		
		yield return new WaitForSeconds(3f);
		player.GetComponent<Animator>().Play("ShipTurn");

		yield return new WaitForSeconds(6f);
		finalScreen.SetActive(true);
		yield return null;
	}

	public void StartMeteors() {
		List<TrailerMeteor> meteors = meteorParent.GetComponentsInChildren<TrailerMeteor>().ToList();
		foreach(TrailerMeteor m in meteors) {
			m.Moving = true;
		}
	}

	IEnumerator StartShips() {
		List<TrailerLasership> ships = meteorParent.GetComponentsInChildren<TrailerLasership>().OrderBy(t => t.ArrivalIndex).ToList();
		for(int i = 0; i < ships.Count; i++) {
			if(i%2 != 0) {
				yield return new WaitForSeconds(0.05f);
			}
			ships[i].StartActions();
			yield return new WaitForSeconds(0.75f);
		}
	}

	public void ShieldCollidersActive(bool active) {
		foreach(TrailerShield s in player.GetComponentsInChildren<TrailerShield>()){
			s.SetColliderActive(active);			
		}
	}
}
