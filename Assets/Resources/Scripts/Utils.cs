using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Colors { Red, Green, Blue, Yellow }
public enum GameModes { Campaign, Endless, Beamium }

class Utils {

	public static Color GetColorFromGameColor(Colors gameColor) {
		switch (gameColor) {
			default:
			case Colors.Red:
				return Color.red;
			case Colors.Green:
				return Color.green;
			case Colors.Blue:
				return Color.blue;
			case Colors.Yellow:
				return Color.yellow;
		}
	}

	public static Vector3 AngleToVector(float angle) {
		return new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
	}

	public static float xyToAngle(float x, float y) {
		return Mathf.Atan2(y, x) * Mathf.Rad2Deg;
	}

	public static float VectorToAngle(Vector2 vector) {
		return xyToAngle(vector.x, vector.y);
	}
}

