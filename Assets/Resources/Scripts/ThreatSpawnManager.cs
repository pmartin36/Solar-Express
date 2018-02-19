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
			new OrbitingEnemyParameters(0, Colors.Red, new Vector3(5,2), angle: 0),
			new EMPParameters(0, Colors.Red, new Vector2(-1f,1f)),

			new EMPParameters(8.5f, Colors.Green, new Vector2(-1.5f,0f)),
			new OrbitingEnemyParameters(12, Colors.Green, new Vector3(5,2), angle: 0),

			new EMPParameters(20f, Colors.Yellow, new Vector2(0f,1.5f)),
			new EMPParameters(20f, Colors.Yellow, new Vector2(0f,-1.5f)),
			new OrbitingEnemyParameters(22, Colors.Yellow, new Vector3(5,2), angle: 0),

			new EMPParameters(32.5f, Colors.Yellow, new Vector2(1f,-1f)),
			new OrbitingEnemyParameters(34, Colors.Green, new Vector3(0,7), angle: 80),		
			new MeteorParameters(39, Colors.Yellow, Utils.AngleToVector(45)*7 + Vector3.up * 0.75f, 45, 3.5f),
			new MeteorParameters(39, Colors.Yellow, Utils.AngleToVector(45)*7, 45, 4f),
			new MeteorParameters(39, Colors.Yellow, Utils.AngleToVector(45)*7 + Vector3.down * 0.75f, 45, 4.5f),

			new MeteorParameters(44, Colors.Red, Utils.AngleToVector(50)*7, 50, 4f),
			new MeteorParameters(44, Colors.Red, Utils.AngleToVector(130)*7, 130, 3.5f),
			new MeteorParameters(44, Colors.Blue, Utils.AngleToVector(-90)*7, -90, 3.5f),
			new OrbitingEnemyParameters(47, Colors.Blue, new Vector3(6,-2), angle: -40),
			new OrbitingEnemyParameters(47, Colors.Green, new Vector3(6,2), angle: 40),

			new OrbitingEnemyParameters(56, Colors.Red, Utils.AngleToVector(90)*7, angle: 90),
			new EMPParameters(56, Colors.Green, new Vector2(1f,1f)),
			new EMPParameters(59.5f, Colors.Yellow, new Vector2(0.6f,1.4f)),
			new MeteorParameters(62.5f, Colors.Green, Utils.AngleToVector(-90)*7, -90, 3.5f),
			new OrbitingEnemyParameters(64.5f, Colors.Red, Utils.AngleToVector(-90)*7, angle: -90),
			new MeteorParameters(66.5f, Colors.Yellow, Utils.AngleToVector(-90)*7, -90, 4f),

			new EMPParameters(72f, Colors.Blue, new Vector2(1,1f)),
			new OrbitingEnemyParameters(74, Colors.Red, new Vector3(5,2), angle: 0),
			new EMPParameters(75.5f, Colors.Blue, new Vector2(1.5f, 0)),
			new MeteorParameters(82, Colors.Blue, Utils.AngleToVector(90)*7, 90, 5.5f),

			new LevelEnd(88,3500)
		};

		float startTime = 70;
		threats = threats.Select( t => {
			if(t is MeteorParameters) {
				(t as MeteorParameters).Velocity *= 0.6f;
			}
			return t;
		}).
		// comment when level completed
		//Select( t => {
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
