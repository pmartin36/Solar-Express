using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LevelFailMenu : MonoBehaviour {

	public float MinLightRadius, MaxLightRadius;
	public Color InnerColor, OuterColor;

	public float ScreenDistortMagnitude;
	public Material ScreenDistort;

	private Image image;
	private Animator anim;

	Button[] buttons;
	TMP_Text MissionFailedText;
	public float ButtonAlpha;

	// Use this for initialization
	void Start () {
		image = image ?? GetComponent<Image>();
		anim = anim ?? GetComponent<Animator>();

		buttons = GetComponentsInChildren<Button>();
		MissionFailedText = GetComponentsInChildren<TMP_Text>().First( t => t.tag == "MissionStatusText");

		UpdateVals();
	}

	private void UpdateVals() {
		image.material.SetFloat("_MinLightRadius", MinLightRadius);
		image.material.SetFloat("_MaxLightRadius", MaxLightRadius);
		image.material.SetColor("_InnerColor", InnerColor);
		image.material.SetColor("_OuterColor", OuterColor);

		ScreenDistort.SetFloat("_Magnitude", ScreenDistortMagnitude);

		Color c = Color.Lerp(Color.clear, Color.white, ButtonAlpha);
		foreach (Button b in buttons) {
			var colors = b.colors;
			colors.normalColor = c;
			b.colors = colors;
		}
		MissionFailedText.color = c;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateVals();
	}

	public void DeathAnimationCompleted() {
		Camera.main.GetComponent<CameraController>().PostMaterial = null;

		//enable buttons
		foreach (Button b in buttons) {
			b.interactable = true;
		}

		GameManager.Instance.MenuParticles.SetActive(true);
	}

	private void OnEnable() {
		Camera.main.GetComponent<CameraController>().PostMaterial = ScreenDistort;
	}
}
