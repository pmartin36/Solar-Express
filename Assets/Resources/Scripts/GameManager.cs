using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

	public bool MenuOpen { get; set; }
	public CameraController MainCameraController { get; set; }
	public Ship PlayerShip;

	public void ProcessInputs(InputPackage p) {
		PlayerShip.Rotate(p);
	}
}