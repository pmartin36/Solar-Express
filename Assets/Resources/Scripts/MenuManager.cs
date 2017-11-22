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

	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = false;
		GameManager.Instance.RegisterContextManager(this);	
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

	IEnumerator ToLevelSelect() {	
		yield return CloseHomeScreen();
		yield return OpenLevelSelect(); 
	}
}
