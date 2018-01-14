using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : ContextManager {

	public GameObject HomeScreen;
	public GameObject LevelSelectScreen;

	public Popup ActivePopup;
	public RectTransform ActivePopupRectTransform;

	public LevelSelector LevelSelector;
	public AudioClip MenuMusic;

	private AudioSource audio;

	public override void Awake() {
		base.Awake();
	}

	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = false;
		GameManager.Instance.RegisterContextManager(this);	
		GameManager.Instance.MenuParticles.SetActive(true);

		if(GameManager.Instance.MusicManager != null) {
			MusicManager m = GameManager.Instance.MusicManager;
			m.Volume = 0f;
			if(m.PlayingSong != MenuMusic) {
				m.SetPlayingSong(MenuMusic);
			}
			GameManager.Instance.MusicManager.SetVolumeLevelGradual(0.3f, 1f);
		}

		audio = GetComponent<AudioSource>();
		this.AddAudioSource(audio);
		audio.mute = !GameManager.Instance.PlayerInfo.SoundOn;
	}
	
	public void SetActiveScreen(bool homeActive) {
		HomeScreen.SetActive(homeActive);
		LevelSelectScreen.SetActive(!homeActive);
	}

	public void SwitchToLevelSelect() {
		StartCoroutine(ToLevelSelect());
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if(ActivePopup != null) {
				ActivePopup.gameObject.SetActive(false);
				ActivePopup = null;
			}
			else {
				Application.Quit();
			}
		}
		

		if (ActivePopup != null && Input.touchCount > 0) {
			Touch t = Input.GetTouch(0);
			Vector2 touchPosition = t.position;
			
			if( t.phase == TouchPhase.Began &&
				!RectTransformUtility.RectangleContainsScreenPoint( ActivePopupRectTransform, touchPosition, Camera.main) ) {
				ActivePopup.gameObject.SetActive(false);
				ActivePopup = null;
			}

			//if( touchPosition.x < ActivePopupRect.xMin ||
			//	touchPosition.x > ActivePopupRect.xMax ||
			//	touchPosition.y < ActivePopupRect.yMin ||
			//	touchPosition.y > ActivePopupRect.yMax) {

			//	ActivePopup.gameObject.SetActive(false);
			//	ActivePopup = null;

			//}
		}
	}

	IEnumerator OpenLevelSelect() {
		float startTime = Time.time;
		float ttime = 1f;

		List<Button> lsbuttons = LevelSelectScreen.GetComponentsInChildren<Button>().Where(t => t.tag == "MainMenuButton").ToList();
		foreach (Button b in lsbuttons) {
			var bc = b.colors;
			bc.normalColor = new Color(1, 1, 1, 0);
			b.colors = bc;
		}
		HomeScreen.SetActive(false);

		if (GameManager.Instance.FirstTimePlaying) {
			GameManager.Instance.SwitchLevels(Utils.StoryScene);
		}
		else {
			LevelSelectScreen.SetActive(true);

			startTime = Time.time;

			while (Time.time - startTime < ttime + Time.deltaTime) {
				float jTime = (Time.time - startTime) / ttime;
				foreach (Button b in lsbuttons) {
					var bc = b.colors;
					Color c = bc.normalColor;
					c.a = Mathf.Lerp(0, 1, jTime);
					bc.normalColor = c;
					b.colors = bc;
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void OpenPopup(Popup popup) {
		if(popup == null || popup == ActivePopup) return;
		if(ActivePopup != null) {
			ActivePopup.gameObject.SetActive(false);
		}

		ActivePopup = popup;	
		popup.gameObject.SetActive(true);
		ActivePopupRectTransform = popup.GetComponent<RectTransform>();
	}

	IEnumerator CloseHomeScreen() {
		float startTime = Time.time;
		float ttime = 1f;

		List<Button> homebuttons = HomeScreen.GetComponentsInChildren<Button>().Where(t => t.tag == "MainMenuButton").ToList();
		foreach (Button b in homebuttons) {
			b.interactable = false;
		}

		if (GameManager.Instance.FirstTimePlaying) {
			GameManager.Instance.MusicManager.SetVolumeLevelGradual(0, ttime);
		}

		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;
			foreach (Button b in homebuttons) {
				var bc = b.colors;
				Color c = bc.disabledColor;
				c.a = Mathf.Lerp(1, 0, jTime);
				bc.disabledColor = c;
				b.colors = bc;
			}
			yield return new WaitForEndOfFrame();
		}

		foreach (Button b in homebuttons) {
			b.interactable = true;
		}
	}

	public void CloseMenuStartPlay(GameModes gameMode) {
		StopCoroutine(CloseHomeScreen());

		//determine new scene index based on game mode and level selected

		//start transition
		StartCoroutine(CloseMenu(Utils.LevelSceneFromLevel(0)));
	}

	IEnumerator CloseMenu(int newSceneIndex) {		
		float startTime = Time.time;
		float ttime = 2f;
		List<Button> lsbuttons = LevelSelectScreen.GetComponentsInChildren<Button>().Where(t => t.tag == "MainMenuButton").ToList();

		foreach(Button b in lsbuttons) { b.interactable = false; }
		yield return new WaitForEndOfFrame();

		Vector3 startCameraPosition = Vector3.back*10;
		Vector3 endCameraPosition = new Vector3(0, LevelSelector.transform.position.y+0.04f, -10);
		Color endColor = new Color(0, 8f/255f, 21f/255f);

		Image scrollviewimage = LevelSelector.GetComponent<Image>();

		//stop music, start thruster
		AudioSource audio = GetComponent<AudioSource>();
		audio.Play();
		StartCoroutine(SetVolumeGradual(audio,0));
		GameManager.Instance.MusicManager.SetVolumeLevelGradual(0f, 2f);

		while (Time.time - startTime < ttime + Time.deltaTime) {
			float jTime = (Time.time - startTime) / ttime;

			Camera.main.transform.position = Vector3.Lerp( startCameraPosition, endCameraPosition, jTime);
			Camera.main.orthographicSize = Mathf.Lerp(5,0.7f,jTime);
			Camera.main.backgroundColor = Color.Lerp(Color.black, endColor, jTime);

			LevelSelector.SetSelectedRingAlpha( Mathf.Lerp(0,0.5f,jTime) );
			LevelSelector.SetElementsAlpha2( Mathf.Lerp(1, 0, jTime) );

			scrollviewimage.color = new Color(endColor.r, endColor.g, endColor.b, Mathf.Lerp(0,1,jTime));

			yield return new WaitForEndOfFrame();
		}

		GameManager.Instance.SwitchLevels(newSceneIndex);
	}

	IEnumerator SetVolumeGradual(AudioSource audio, float end) {
		float start = audio.volume;
		float startTime = Time.time;
		float time = 2f;

		while (Time.time - startTime < time + 0.1f) {
			audio.volume = Mathf.Lerp(start, end, (Time.time - startTime) / time);
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator ToLevelSelect() {	
		yield return CloseHomeScreen();
		yield return OpenLevelSelect(); 
	}
}
