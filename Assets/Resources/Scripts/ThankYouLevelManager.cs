using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ThankYouLevelManager : LevelManager {

	public TMP_Text Text;
	public Button MenuButton;

	protected override IEnumerator LoadLevel() {
		SetMenuState();

		yield return ShowText(1f);
		yield return new WaitForSeconds(4f);

		yield return  SwitchText("Thank you\nfor playing!",1f);
		yield return new WaitForSeconds(3f);

		yield return SwitchText("If you enjoyed\nthe game, please\nconsider rating it!", 1f);
		yield return new WaitForSeconds(4f);

		yield return SwitchText("Game By:\nPaul Martin\n@MoonlightMade", 1f);
		yield return new WaitForSeconds(4f);

		yield return SwitchText("Music By:\nAntti Luode\n@aluode", 1f);
		yield return new WaitForSeconds(4f);
		yield return HideText(1f);
		
		yield return new WaitForSeconds(1f);
		yield return ShowMenuButton(1f);

		yield return null;
	}

	public override void ProcessInputs(InputPackage p) {
		return;
	}

	private IEnumerator ShowMenuButton(float showTime) {
		float startTime = Time.time;
		MenuButton.interactable = true;
		while( Time.time - startTime < showTime + Time.deltaTime) {
			Color c = Color.Lerp(Color.clear, Color.white, (Time.time - startTime) / showTime);
			ColorBlock cb = MenuButton.colors;
			cb.normalColor = c;
			MenuButton.colors = cb;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator SwitchText(string newText, float showhideTime) {
		yield return HideText(showhideTime);
		yield return new WaitForSeconds(1f);
		setText(newText);
		yield return ShowText(showhideTime);
	}

	private void setText(string newText) {
		Text.text = newText;
		RectTransform t = Text.GetComponent<RectTransform>();
		t.sizeDelta = new Vector2(650, 100 * Text.text.Split('\n').Length);
	}

	private IEnumerator ShowText(float jTime) {
		float startTime = Time.time;
		Color topColor = Color.clear;
		Color bottomColor = Color.clear;
		do {
			float ttime = (Time.time - startTime) / jTime;
			topColor = Color.Lerp( Color.clear, Color.white, ttime);
			bottomColor = Color.Lerp(Color.clear, Color.white, ttime - 0.4f);
			Text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);
			yield return new WaitForEndOfFrame();
		} while( bottomColor.a < 1f );
	}

	private IEnumerator HideText(float jTime) {
		float startTime = Time.time;
		Color topColor = Color.clear;
		Color bottomColor = Color.clear;
		do {
			float ttime = (Time.time - startTime) / jTime;
			topColor = Color.Lerp(Color.white, Color.clear, ttime - 0.4f);
			bottomColor = Color.Lerp(Color.white, Color.clear, ttime);
			Text.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);
			yield return new WaitForEndOfFrame();
		} while (topColor.a > 0f);
	}
}
