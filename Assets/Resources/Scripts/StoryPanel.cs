using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StoryPanel : MonoBehaviour {

	private Image TalkingSun;
	public Image ShipImage, Finger;

	private TMP_Text Dialog;
	private Button NextButton;
	private Image Border;

	bool NextDialog;
	private AudioSource audio;

	// Use this for initialization
	void Start () {
		var images = GetComponentsInChildren<Image>();
		TalkingSun = images.First( g => g.tag == "Planet" );
		ShipImage = images.First( g => g.tag == "Core");
		Finger = images.First(g => g.tag == "PointDisplay");

		Dialog = GetComponentsInChildren<TMP_Text>().First( g => g.tag != "MainMenuButton" );
		NextButton = GetComponentInChildren<Button>();
		Border = GameObject.FindGameObjectWithTag("Popup").GetComponent<Image>();

		GameManager.Instance.FirstTimePlaying = false;

		audio = GetComponent<AudioSource>();
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;

		StartCoroutine(Action());
	}
	
	// Update is called once per frame
	void Update () {
		TalkingSun.transform.Rotate(new Vector3(0,0,-5*Time.deltaTime));
	}

	public void NextDialogClicked() {
		NextDialog = true;
	}

	IEnumerator WalkThroughText(string text, float speed) {
		for(int i = 0; i < text.Length; i++) {
			char c = text[i];
			Dialog.text = Dialog.text + c;
			float waitTime = c == '.' ? 6f / speed : 1f / speed;
			yield return new WaitForSeconds(waitTime);
		}
	}

	IEnumerator TransitionImageSerial(Image[] images, Color start, Color end, float jTime) {
		float startTime = Time.time;
		while ((Time.time - startTime) < jTime + Time.deltaTime) {
			Color c = Color.Lerp(start, end, (Time.time - startTime) / jTime);
			foreach(Image i in images) {
				i.color = c;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator TransitionImageSerial(Image[] images, Color start, Color[] end, float jTime) {
		float startTime = Time.time;
		while ((Time.time - startTime) < jTime + Time.deltaTime) {		
			for (int i = 0; i < images.Length; i++) { 
				Image image = images[i];
				Color c = Color.Lerp(start, end[i], (Time.time - startTime) / jTime);
				image.color = c;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator TransitionImageSerial(Image[] images, Color[] start, Color end, float jTime) {
		float startTime = Time.time;
		while ((Time.time - startTime) < jTime + Time.deltaTime) {
			for (int i = 0; i < images.Length; i++) {
				Image image = images[i];
				Color c = Color.Lerp(start[i], end, (Time.time - startTime) / jTime);
				image.color = c;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SpecialActionBefore(int i) {
		switch (i) {
			case 2:
				break;
			case 3:
				StartCoroutine(TransitionImageSerial(new[] { TalkingSun }, new Color(1, 1, 1, 0.4f), Color.white, 1f));
				break;
			case 4:
				//appear finger, start animation
				yield return StartCoroutine(TransitionImageSerial(new[] { Finger }, Color.clear, Color.white, 1f));
				GetComponent<Animator>().Play("StoryControls");
				break;
			case 5:
				GetComponent<Animator>().enabled = false;
				Finger.gameObject.SetActive(false);

				//var shipImages = ShipImage.GetComponentsInChildren<Image>().ToList();
				//shipImages.Add(Finger);
				//var colors = shipImages.Select(g => {
				//	Color c = g.color;
				//	c.a = 1;
				//	return c;
				//});

				//yield return StartCoroutine(TransitionImageSerial(shipImages.ToArray(), colors.ToArray(), Color.clear, 1f));
				//yield return StartCoroutine(TransitionImageSerial(new[] { TalkingSun }, Color.clear, Color.white, 1f));
				break;
			default:
				break;
		}
	}

	IEnumerator SpecialActionDuring(int i) {
		switch (i) {
			case 2:
				StartCoroutine(TransitionImageSerial(new[] { ShipImage }, Color.clear, Color.white, 1f));
				break;
			case 3:
				//yield return StartCoroutine(TransitionImageSerial(new[] { TalkingSun }, Color.white, Color.clear, 0.5f));
				yield return new WaitForSeconds(2.5f);
				var storycore = ShipImage.GetComponent<StoryCore>();
				storycore.SpawnShields();
				break;
			default:
				break;
		}
	}

	IEnumerator Action() {
		StartCoroutine(TransitionImageSerial(new [] { TalkingSun }, Color.clear, new Color(1,1,1,0.4f), 2f));
		yield return StartCoroutine(TransitionImageSerial(new[] { Border }, Color.clear, Color.white, 2f));

		var dialogPieces = new [] {
			"Welcome to the Sun. I am the Essence of Color. Thank you for your quick response.",
			"Color is disappearing from the Solar System and I am not powerful enough to restore it to it's former vibrancy.",
			"I have imbued your ship with my power. This power allows you to restore color to any celestial body you visit.",
			"To protect your ship, I have equipped it with 4 different colored shields.",
			"These shields can be rotated around your ship's core. Use them to block threats with a matching color.",
			"There isn't any more time!  Visit all the planets and stars to restore color before it's too late."
		};

		for(int i = 0; i < dialogPieces.Count(); i++) {
			Dialog.text = "";

			yield return StartCoroutine(SpecialActionBefore(i));
			StartCoroutine(SpecialActionDuring(i));

			string s = dialogPieces[i];
			yield return WalkThroughText(s, 20f);
			NextButton.interactable = true;
			yield return new WaitUntil(() => NextDialog);
			NextDialog = false;
			NextButton.interactable = false;
		}

		float startTime = Time.time;
		float volume = audio.volume;
		Color startColor = Dialog.color;
		StoryCore core = GetComponentInChildren<StoryCore>();

		StartCoroutine(TransitionImageSerial(new [] { TalkingSun, Border }, Color.white, Color.clear, 1f));
		core.Despawn();
		while(Time.time - startTime < 1f + Time.deltaTime) {
			float time = (Time.time - startTime);
			audio.volume = Mathf.Lerp(volume, 0, time);
			Dialog.color = Color.Lerp(startColor, Color.clear, time);
			yield return new WaitForEndOfFrame();
		}

		GameManager.Instance.TransitioningToHome = false;
		GameManager.Instance.SwitchLevels(Utils.MenuScene);
	}
}
