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
			new OrbitingEnemyParameters(15, Colors.Red, new Vector3(5,-2)),
			new LevelEnd(20)
		};
		*/
		threats = new List<IThreatParams>() {
			new OrbitingEnemyParameters(0, Colors.Green, Utils.AngleToVector(90)*7, moveSpeed: 1.5f, angle: 75),
			new OrbitingEnemyParameters(0, Colors.Red, Utils.AngleToVector(90)*7, moveSpeed: 1.5f, angle: 105),
			new PointBeamParameters(1, Colors.Green, 5, Vector3.up * 3f),

			new OrbitingEnemyParameters(9, Colors.Blue, Utils.AngleToVector(-45)*7 + Vector3.up * 1.5f, moveSpeed: 1.5f, angle: -45),
			new OrbitingEnemyParameters(9, Colors.Yellow, Utils.AngleToVector(45)*7 + Vector3.left * 1.5f, moveSpeed: 1.5f, angle: 45),
			new OrbitingEnemyParameters(9, Colors.Red, Utils.AngleToVector(135)*7 + Vector3.down * 1.5f, moveSpeed: 1.5f, angle: 135),

			new PointBeamParameters(20, Colors.Red, 2, Vector3.up * 3f),

			new PointBeamParameters(23, Colors.Blue, 2, Utils.AngleToVector(135)*3),
			new PointBeamParameters(23, Colors.Green, 2, Utils.AngleToVector(45)*3),

			new PointBeamParameters(26, Colors.Red, 2, Utils.AngleToVector(180)*3),
			new PointBeamParameters(26, Colors.Blue, 2, Utils.AngleToVector(0)*3),

			new PointBeamParameters(29, Colors.Blue, 2, Utils.AngleToVector(-45)*3),
			new PointBeamParameters(29, Colors.Green, 2, Utils.AngleToVector(-135)*3),

			new PointBeamParameters(32, Colors.Red, 2, Vector3.down * 3f),

			new MeteorParameters(36, Colors.Red, Utils.AngleToVector(90)*7, 90, 6f),

			new MeteorParameters(39, Colors.Blue, Utils.AngleToVector(135)*7, 135, 6f),
			new MeteorParameters(39, Colors.Green, Utils.AngleToVector(45)*7, 45, 6f),

			new MeteorParameters(42, Colors.Red, Utils.AngleToVector(180)*7, 180, 6f),
			new MeteorParameters(42, Colors.Blue, Utils.AngleToVector(0)*7, 0, 6f),

			new MeteorParameters(45, Colors.Blue, Utils.AngleToVector(-45)*7, -45, 6f),
			new MeteorParameters(45, Colors.Green, Utils.AngleToVector(-135)*7, -135, 6f),

			new MeteorParameters(48, Colors.Red, Utils.AngleToVector(-90)*7, -90, 6f),

			new OrbitingEnemyParameters(52, Colors.Red, Utils.AngleToVector(135)*7, moveSpeed: 2.5f, angle: 135),
			new OrbitingEnemyParameters(61, Colors.Blue, Utils.AngleToVector(-45)*7, moveSpeed: 2.5f, angle: -45),

			new MeteorParameters(71, Colors.Red, Utils.AngleToVector(135)*7, 135, 6f),
			new MeteorParameters(74, Colors.Red, Utils.AngleToVector(180)*7, 180, 6f),
			new MeteorParameters(77, Colors.Red, Utils.AngleToVector(-135)*7, -135, 6f),

			new MeteorParameters(80, Colors.Blue, Utils.AngleToVector(-45)*7, -45, 6f),
			new MeteorParameters(83, Colors.Blue, Utils.AngleToVector(0)*7, 0, 6f),
			new MeteorParameters(86, Colors.Blue, Utils.AngleToVector(45)*7, 45, 6f),

			new LevelEnd(90, 2000)
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
