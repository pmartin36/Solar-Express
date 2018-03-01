using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatSpawnManager : MonoBehaviour {

	public List<IThreatParams> threats;
	int threatIndex = 0;
	float time;

	public void Init(int levelNumber) {
		time = 0;
		if (GameManager.Instance.ThreatParams.ContainsKey(levelNumber)) {
			threats = GameManager.Instance.ThreatParams[levelNumber];
			return;
		}

		//comment after level generation
		GenerateLevel(levelNumber);	
	}

	public void GenerateLevel(int levelNumber) {
		/*
		threats = new List<IThreatParams>() {
			new LaserShipParameters(0, Colors.Blue, 4.5f, 135, 8, 7),
			new MeteorParameters(3, Colors.Blue, Utils.AngleToVector(-70)*7, -70, 3.5f),
			new PointBeamParameters(12, Colors.Green, 10, Vector3.one * 2.5f),
			new EMPParameters(10, Colors.Yellow, new Vector2(-1.5f,1f)),
			new OrbitingEnemyParameters(0, Colors.Green, Utils.AngleToVector(90)*7, moveSpeed: 1.5f, angle: 75),
			new LevelEnd(20)
		};
		*/
		threats = new List<IThreatParams>() {
			new EMPParameters(0, Colors.Red, new Vector2(0f,-1.5f)),

			new PointBeamParameters(2, Colors.Blue, 4, new Vector3(3,-3)),
			new PointBeamParameters(2, Colors.Green, 4, new Vector3(-3,-3)),

			new EMPParameters(4, Colors.Blue, new Vector2(1.5f,0)),
			new EMPParameters(4, Colors.Red, new Vector2(-1.5f,0)),

			new PointBeamParameters(6, Colors.Blue, 4, new Vector3(-3,3)),
			new PointBeamParameters(6, Colors.Green, 4, new Vector3(3,3)),

			new EMPParameters(8, Colors.Red, new Vector2(0,1.5f)),
			new LaserShipParameters(10, Colors.Red, 3.5f, 90, 2, 8),

			new OrbitingEnemyParameters(12, Colors.Green, Utils.AngleToVector(45)*7, moveSpeed: 2.5f, angle: 45),
			new OrbitingEnemyParameters(12, Colors.Blue, Utils.AngleToVector(135)*7, moveSpeed: 2.5f, angle: 135),

			new LaserShipParameters(14, Colors.Red, 3.5f, 180, 2, 8),
			new LaserShipParameters(14, Colors.Blue, 3.5f, 0, 2, 8),

			new OrbitingEnemyParameters(16, Colors.Green, Utils.AngleToVector(-135)*7, moveSpeed: 2.5f, angle: -135),
			new OrbitingEnemyParameters(16, Colors.Blue, Utils.AngleToVector(-45)*7, moveSpeed: 2.5f, angle: -45),

			new LaserShipParameters(18, Colors.Red, 3.5f, -90, 2, 8),

			new MeteorParameters(24, Colors.Blue, Utils.AngleToVector(-45)*7, -45, 6f),
			new MeteorParameters(24, Colors.Green, Utils.AngleToVector(-135)*7, -135, 6f),

			new MeteorParameters(28, Colors.Green, Utils.AngleToVector(45)*7, 45, 6f),
			new MeteorParameters(28, Colors.Blue, Utils.AngleToVector(135)*7, 135, 6f),

			new PointBeamParameters(34, Colors.Blue, 4, new Vector3(-3,3)),
			new PointBeamParameters(34, Colors.Green, 4, new Vector3(3,3)),

			new PointBeamParameters(38, Colors.Blue, 4, new Vector3(3,-3)),
			new PointBeamParameters(38, Colors.Green, 4, new Vector3(-3,-3)),

			new MeteorParameters(44, Colors.Green, Utils.AngleToVector(135)*7, 135, 3f),
			new MeteorParameters(44, Colors.Yellow, Utils.AngleToVector(125)*7, 135, 3.5f, points: 0),
			new MeteorParameters(44, Colors.Blue, Utils.AngleToVector(145)*7, 135, 4f, points: 0),

			new MeteorParameters(45.5f, Colors.Red, Utils.AngleToVector(130)*7, 135, 3f),
			new MeteorParameters(45.5f, Colors.Green, Utils.AngleToVector(120)*7, 135, 2.5f, points: 0),
			new MeteorParameters(45.5f, Colors.Yellow, Utils.AngleToVector(140)*7, 135, 3.5f, points: 0),

			new MeteorParameters(47, Colors.Blue, Utils.AngleToVector(140)*7, 135, 4f),
			new MeteorParameters(47, Colors.Green, Utils.AngleToVector(130)*7, 135, 2.5f),
			new MeteorParameters(47, Colors.Red, Utils.AngleToVector(150)*7, 135, 3f, points: 0),

			new MeteorParameters(48.5f, Colors.Yellow, Utils.AngleToVector(138)*7, 135, 2.5f),
			new MeteorParameters(48.5f, Colors.Blue, Utils.AngleToVector(122)*7, 135, 3.5f, points: 0),
			new MeteorParameters(48.5f, Colors.Red, Utils.AngleToVector(145)*7, 135, 3f, points: 0),

			new OrbitingEnemyParameters(54, Colors.Yellow, Utils.AngleToVector(135)*7, moveSpeed: 3f, angle: 135),
			new PointBeamParameters(54, Colors.Yellow, 4, new Vector3(-3,-3)),
			new OrbitingEnemyParameters(58f, Colors.Yellow, Utils.AngleToVector(-45)*7, moveSpeed: 3f, angle: -45),
			new PointBeamParameters(58, Colors.Green, 4, new Vector3(-3,-3)),
			new OrbitingEnemyParameters(62, Colors.Yellow, Utils.AngleToVector(135)*7, moveSpeed: 2f, angle: 135),
			new MeteorParameters(64, Colors.Blue, Utils.AngleToVector(150)*7, 150, 4.5f),
			new OrbitingEnemyParameters(68f, Colors.Yellow, Utils.AngleToVector(-45)*7, moveSpeed: 2f, angle: -45),
			new MeteorParameters(72, Colors.Red, Utils.AngleToVector(-150)*7, -150, 4.5f),
			new OrbitingEnemyParameters(72f, Colors.Yellow, Utils.AngleToVector(135)*7, moveSpeed: 1f, angle: 135),
			new OrbitingEnemyParameters(77f, Colors.Blue, Utils.AngleToVector(135)*7, moveSpeed: 1.2f, angle: 135),
			new OrbitingEnemyParameters(82f, Colors.Yellow, Utils.AngleToVector(-45)*7, moveSpeed: 1f, angle: -45),
			new OrbitingEnemyParameters(82f, Colors.Blue, Utils.AngleToVector(-135)*7, moveSpeed: 1f, angle: -135),
			new OrbitingEnemyParameters(82f, Colors.Green, Utils.AngleToVector(135)*7, moveSpeed: 1f, angle: 135),
			new OrbitingEnemyParameters(82f, Colors.Red, Utils.AngleToVector(45)*7, moveSpeed: 1f, angle: 45),

			new EMPParameters(90, Colors.Red, new Vector2(3,3)),
			new EMPParameters(90, Colors.Blue, new Vector2(3,-3f)),
			new EMPParameters(90, Colors.Green, new Vector2(-3,3)),
			new EMPParameters(90, Colors.Yellow, new Vector2(-3,-3)),

			new PointBeamParameters(91, Colors.Yellow, 4, new Vector3(0,3)),
			new PointBeamParameters(91, Colors.Red, 4, new Vector3(-3,0)),
			new PointBeamParameters(91, Colors.Green, 4, new Vector3(0,-3)),
			new PointBeamParameters(91, Colors.Blue, 4, new Vector3(3,0)),

			new LevelEnd(95,1000)
		};

		float startTime = 0;
		threats = threats.Select( t => {
			if(t is MeteorParameters) {
				(t as MeteorParameters).Velocity *= 0.6f;
			}
			return t;
		}).
		// comment when level completed
		//Select(t => {
		//	if (t is MeteorParameters) {
		//		(t as MeteorParameters).Damage = 0;
		//	}
		//	t.SpawnTime -= startTime;
		//	return t;
		//}).
		Where( t => t.SpawnTime >= 0)
		.ToList();
		
		//only uncomment when YOU HAVE CHECKED THE LEVEL NUMBER IS NOT EXISTING YET
		//SaveThreats(levelNumber);
	}

	public void SaveThreats(int levelNumber) {
		var tp = GameManager.Instance.ThreatParams;
		if(tp.ContainsKey(levelNumber)) {
			tp[levelNumber] = threats;
		}
		else {
			tp.Add(levelNumber, threats);
		}
		Serializer<Dictionary<int, List<IThreatParams>>>.Serialize(tp,"Threats.bin");
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		while(threatIndex < threats.Count && threats[threatIndex].SpawnTime < time) {
			IThreatParams t = threats[threatIndex];

			if(t is EMPParameters) {
				EMPFactory.Create(t as EMPParameters);
			}
			else if(t is LaserShipParameters) {
				LaserShipFactory.Create(t as LaserShipParameters);
			}
			else if(t is MeteorParameters) {
				MeteorFactory.Create(t as MeteorParameters);
			}
			else if(t is OrbitingEnemyParameters) {
				OrbitingEnemyFactory.Create(t as OrbitingEnemyParameters);
			}
			else if(t is PointBeamParameters) {
				PointBeamFactory.Create(t as PointBeamParameters);
			}
			else if(t is LevelEnd) {
				var lm = (GameManager.Instance.ContextManager as LevelManager);
				lm.TotalAvailablePoints += (t as LevelEnd).ExtraPoints;
				(GameManager.Instance.ContextManager as LevelManager).BeginEndLevel();
			}

			threatIndex++;
		}
	}
}
