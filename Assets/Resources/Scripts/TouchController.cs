using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

	float touchDownAngle;
	bool touchDown;
	float playerStartAngle;

	private float lastUpdateAngle, lastUpdateRotation, freeRotationAmount;
	private Vector2 lastTouchPosition;
	private float lastTouchDistance;
	private float unmovedTime;

	ParticleSystem.EmissionModule fp_em;

	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = false;
		fp_em = (GameManager.Instance.ContextManager as LevelManager).FingerParticles.emission;
	}
	
	// Update is called once per frame
	void Update () {
		var fp = (GameManager.Instance.ContextManager as LevelManager).FingerParticles;
		float angleDiff = 0;
		Vector2 touchPosition = Vector2.zero;
		freeRotationAmount /= (1+Time.deltaTime/2f);	

		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch(0);
			freeRotationAmount = 0f;

			touchPosition = t.position;
			touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.transform.position.z));			

			if(touchDown) {
				float tangle = Vector3.Angle(Vector3.right, touchPosition);
				tangle *= -Mathf.Sign(Vector3.Cross(Vector3.right, touchPosition).z);

				angleDiff += (touchDownAngle - tangle);				

				float dist = (touchPosition - lastTouchPosition).magnitude;
				if(dist >= 0.07f || unmovedTime > 0.1f) {
					unmovedTime = 0f;
					lastTouchDistance = (touchPosition - lastTouchPosition).magnitude;
					lastTouchPosition = touchPosition;

					lastUpdateRotation = (angleDiff - lastUpdateAngle) * Time.deltaTime;
					lastUpdateAngle = angleDiff;
					Debug.Log(touchPosition);
				}
				else {
					unmovedTime += Time.deltaTime;
				}

				fp.transform.position = touchPosition;			
			}
			else {
				// process input as input for ship
				touchDownAngle = Vector3.Angle(Vector3.right, touchPosition);
				touchDownAngle *= -Mathf.Sign(Vector3.Cross(Vector3.right, touchPosition).z);
				touchDown = true;
				playerStartAngle = (GameManager.Instance.ContextManager as LevelManager).PlayerShip.transform.localRotation.eulerAngles.z;
				fp_em.enabled = true;

				lastUpdateAngle = 0f;
				lastUpdateRotation = 0f;
				lastTouchDistance = 0f;
				unmovedTime = 0f;

				fp.gameObject.SetActive(false);
				fp.transform.position = touchPosition;
				fp.gameObject.SetActive(true);
			}
			angleDiff += playerStartAngle;			
		}
		else {
			HandleTouchup();
		}

		(GameManager.Instance.ContextManager as LevelManager).ProcessInputs(new InputPackage() {
			AngleDiff = angleDiff,
			TouchPosition = touchPosition,
			FreeRotation = freeRotationAmount,
			Touchdown = touchDown
		});
	}
	
	public void HandleTouchup() {
		if (touchDown) {
			float ratio = Mathf.Clamp01(1 - Mathf.Abs(Mathf.Abs(lastTouchPosition.x) - Mathf.Abs(lastTouchPosition.y)));
			float moddedRotation = lastUpdateRotation + 0.01f * lastTouchDistance;// * Mathf.Sign(lastUpdateRotation) * ratio;
			Debug.Log(moddedRotation + " " + lastTouchDistance + " " + lastUpdateRotation);
			if (Mathf.Abs(moddedRotation) >= 0.015f) {
				freeRotationAmount = Mathf.Clamp(moddedRotation * 60f, -12, 12);
			}
		}
		touchDown = false;
		fp_em.enabled = false;
	}
}
