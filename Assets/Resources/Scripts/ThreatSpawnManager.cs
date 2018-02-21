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
			new MeteorParameters(0, Colors.Green, new Vector3(5,5,0), 45),
			new LaserShipParameters(0, Colors.Blue, 2, 135),
			new PointBeamParameters(10, Colors.Yellow, 10, Vector3.one * -3),
			new EMPParameters(10, Colors.Yellow, new Vector2(-1.5f,1f)),
			new OrbitingEnemyParameters(15, Colors.Red, new Vector3(5,-2)),
			new LevelEnd(20)
		};
		*/
		threats = new List<IThreatParams>() {
			new OrbitingEnemyParameters(0, Colors.Red, new Vector3(0,7), angle: 95f),
			new PointBeamParameters(1, Colors.Red, 5, Utils.AngleToVector(150)*3),

			new OrbitingEnemyParameters(6, Colors.Blue, Utils.AngleToVector(20)*7, angle: 20f),
			new PointBeamParameters(7, Colors.Red, 5, Utils.AngleToVector(-60)*3),

			new LaserShipParameters(9.7f, Colors.Red, 4, 85, 4, 2.8f),
			new OrbitingEnemyParameters(15, Colors.Red, Utils.AngleToVector(95)*7, angle: 95),
			new OrbitingEnemyParameters(22.5f, Colors.Red, Utils.AngleToVector(-145)*7, angle: -145),

			new PointBeamParameters(30, Colors.Red, 2, Utils.AngleToVector(60)*3),
			new PointBeamParameters(32, Colors.Green, 2, Utils.AngleToVector(-160)*3),
			new PointBeamParameters(33, Colors.Blue, 2, Utils.AngleToVector(-60)*3),
			new PointBeamParameters(34, Colors.Yellow, 2, Utils.AngleToVector(20)*3),

			new PointBeamParameters(36, Colors.Red, 2, Utils.AngleToVector(120)*3),
			new PointBeamParameters(38, Colors.Green, 2, Utils.AngleToVector(160)*3),
			new PointBeamParameters(40, Colors.Blue, 2, Utils.AngleToVector(-120)*3),
			new OrbitingEnemyParameters(41f, Colors.Red, Utils.AngleToVector(90)*7, angle: 90),
			new PointBeamParameters(42, Colors.Yellow, 2, Utils.AngleToVector(-30)*3),

			new PointBeamParameters(44, Colors.Yellow, 2, Utils.AngleToVector(60)*3),
			new OrbitingEnemyParameters(46f, Colors.Red, Utils.AngleToVector(-90)*7, angle: -90),
			new PointBeamParameters(46, Colors.Red, 2, Utils.AngleToVector(-160)*3),
			new PointBeamParameters(48, Colors.Yellow, 2, Utils.AngleToVector(-120)*3),
			new PointBeamParameters(50, Colors.Red, 2, Utils.AngleToVector(20)*3),

			new LaserShipParameters(50, Colors.Blue, 3.5f, 45, 5, 4),
			new OrbitingEnemyParameters(56f, Colors.Blue, Utils.AngleToVector(45)*7, angle: 45),
			new OrbitingEnemyParameters(65f, Colors.Blue, Utils.AngleToVector(45)*7, angle: 45),

			new LaserShipParameters(71, Colors.Green, 3f, 180, 3, 3),
			new LaserShipParameters(73, Colors.Red, 4f, 135, 2, 4),

			new LevelEnd(95)
		};

		float startTime = 0;
		threats = threats.Select( t => {
			if(t is MeteorParameters) {
				(t as MeteorParameters).Velocity *= 0.6f;
			}
			return t;
		}).
		//comment when level completed
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
