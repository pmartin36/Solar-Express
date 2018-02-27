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
			new LaserShipParameters(0, Colors.Blue, 4f, 90, 4, 6),
			new EMPParameters(0, Colors.Red, new Vector2(-1.5f,0f)),
			new OrbitingEnemyParameters(5.5f, Colors.Red, Utils.AngleToVector(170)*7, moveSpeed: 2f, angle: 160),
			new EMPParameters(10, Colors.Blue, new Vector2(1f,1f)),
			new OrbitingEnemyParameters(13, Colors.Green, Utils.AngleToVector(-105)*7, moveSpeed: 2f, angle: -90),

			new OrbitingEnemyParameters(21, Colors.Green, Utils.AngleToVector(180)*7, moveSpeed: 2f, angle: 175),
			new OrbitingEnemyParameters(22f, Colors.Yellow, Utils.AngleToVector(0)*7, moveSpeed: 2f, angle: -5),
			new EMPParameters(22, Colors.Red, new Vector2(1.1f, -0.9f)),

			new LaserShipParameters(30, Colors.Red, 3f, 60, 2, 1.5f),

			new LaserShipParameters(37, Colors.Red, 3f, 90, 3, 4f),
			new LaserShipParameters(37, Colors.Green, 3f, 180, 3, 5f),
			new LaserShipParameters(37, Colors.Blue, 3f, -90, 3, 6f),
			new LaserShipParameters(37, Colors.Yellow, 3f, 0, 3, 7f),

			new EMPParameters(39, Colors.Green, new Vector2(-0.9f, 1.1f)),
			new EMPParameters(39, Colors.Blue, new Vector2(-1.1f, -0.9f)),
			new EMPParameters(39, Colors.Yellow, new Vector2(0.9f, -1.1f)),

			new EMPParameters(47, Colors.Green, new Vector2(-1.1f, -0.9f)),
			new EMPParameters(47, Colors.Blue, new Vector2(0.9f, -1.1f)),
			new EMPParameters(47, Colors.Yellow, new Vector2(1.1f, 0.9f)),

			new OrbitingEnemyParameters(60, Colors.Red, Utils.AngleToVector(-30)*7, moveSpeed: 4f, angle: -30),
			new OrbitingEnemyParameters(64, Colors.Green, Utils.AngleToVector(60)*7, moveSpeed: 4f, angle: 60),
			new OrbitingEnemyParameters(67, Colors.Blue, Utils.AngleToVector(150)*7, moveSpeed: 4f, angle: 150),
			new OrbitingEnemyParameters(70, Colors.Yellow, Utils.AngleToVector(-60)*7, moveSpeed: 4f, angle: -60),

			new LaserShipParameters(74, Colors.Red, 3f, 90, 1, 11f),
			new LaserShipParameters(74, Colors.Green, 3f, 180, 1, 11f),
			new LaserShipParameters(74, Colors.Blue, 3f, -90, 1, 11),
			new LaserShipParameters(74, Colors.Yellow, 3f, 0, 1, 11),

			new EMPParameters(76, Colors.Red, new Vector2(0f, 1.42f)),

			new EMPParameters(78, Colors.Green, new Vector2(1f, 1f)),
			new EMPParameters(78, Colors.Blue, new Vector2(-1f, 1f)),

			new EMPParameters(80, Colors.Blue, new Vector2(1.42f, 0f)),
			new EMPParameters(80, Colors.Red, new Vector2(-1.42f, 0f)),

			new EMPParameters(82, Colors.Blue, new Vector2(1f, -1f)),
			new EMPParameters(82, Colors.Green, new Vector2(-1f, -1f)),

			new EMPParameters(84, Colors.Red, new Vector2(0f, -1.42f)),

			new LevelEnd(92, 4000)
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
