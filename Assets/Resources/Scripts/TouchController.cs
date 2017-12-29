using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour {

	float touchDownAngle;
	bool touchDown;
	float playerStartAngle;

	// Use this for initialization
	void Start () {
		Input.multiTouchEnabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		var fp = (GameManager.Instance.ContextManager as LevelManager).FingerParticles;
		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch(0);

			Vector2 touchPosition = t.position;
			touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, Camera.main.transform.position.z));			

			float angleDiff = 0;
			if(touchDown) {
				float tangle = Vector3.Angle(Vector3.right, touchPosition);
				tangle *= -Mathf.Sign(Vector3.Cross(Vector3.right, touchPosition).z);

				angleDiff += (touchDownAngle - tangle);
			}
			else {
				// process input as input for ship
				touchDownAngle = Vector3.Angle(Vector3.right, touchPosition);
				touchDownAngle *= -Mathf.Sign(Vector3.Cross(Vector3.right, touchPosition).z);
				touchDown = true;
				playerStartAngle = (GameManager.Instance.ContextManager as LevelManager).PlayerShip.transform.localRotation.eulerAngles.z;
				fp.gameObject.SetActive(true);
			}
			angleDiff += playerStartAngle;
			fp.transform.position = touchPosition;

			(GameManager.Instance.ContextManager as LevelManager).ProcessInputs(new InputPackage() {
				AngleDiff = angleDiff,
				TouchPosition = touchPosition
			});
		}
		else {
			touchDown = false;
			fp.gameObject.SetActive(false);
		}
	}
	
}
