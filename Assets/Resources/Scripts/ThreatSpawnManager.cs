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
			new OrbitingEnemyParameters(0, Colors.Blue, new Vector3(-7,2), angle: 180),
			new MeteorParameters(3, Colors.Green, new Vector3(-7,0), 180, 3f),

			new MeteorParameters(9, Colors.Yellow, Utils.AngleToVector(135)*7f, 135, 3.8f),
			new MeteorParameters(11, Colors.Red, Utils.AngleToVector(45)*7f, 45, 3.8f),
			new OrbitingEnemyParameters(13, Colors.Yellow, new Vector3(-3,7), angle: 120),

			new OrbitingEnemyParameters(21, Colors.Blue, Utils.AngleToVector(135)*7f, angle: 135),
			new OrbitingEnemyParameters(21, Colors.Green, Utils.AngleToVector(45)*7f, angle: 45),
			new MeteorParameters(24, Colors.Red, Utils.AngleToVector(90)*7f, 90, 3.5f),

			new OrbitingEnemyParameters(31, Colors.Red, Utils.AngleToVector(135)*7f, angle: 135),
			new OrbitingEnemyParameters(31, Colors.Yellow, Utils.AngleToVector(45)*7f, angle: 45),
			new MeteorParameters(36, Colors.Green, Utils.AngleToVector(70)*7f, 70, 3.5f),
			new MeteorParameters(36, Colors.Blue, Utils.AngleToVector(160)*7f, 160, 3.5f),

			new OrbitingEnemyParameters(42, Colors.Blue, Utils.AngleToVector(90)*7f, angle: 90),
			new MeteorParameters(49, Colors.Blue, Utils.AngleToVector(90)*7f, 90, 3f),
			new OrbitingEnemyParameters(50, Colors.Blue, Utils.AngleToVector(-90)*7f, angle: -90),

			new OrbitingEnemyParameters(58, Colors.Yellow, new Vector3(-7,2), angle: 180),
			new OrbitingEnemyParameters(60, Colors.Green, new Vector3(2,7), angle: 90),
			new MeteorParameters(60, Colors.Green, Utils.AngleToVector(30)*7f, 30, 3.8f),

			new OrbitingEnemyParameters(70, Colors.Red, Utils.AngleToVector(45)*7f, angle: 45),
			new MeteorParameters(73, Colors.Yellow, new Vector3(7, 0.75f), 0, 3.2f),
			new MeteorParameters(75, Colors.Green, Utils.AngleToVector(45)*7f, 40, 3.2f),
			new MeteorParameters(77, Colors.Blue, new Vector3(7, 0f), 5, 3.2f),

			new LevelEnd(84)
		};

		//comment when going to create level
		threats = threats.Select( t => {
			if(t is MeteorParameters) {
				//(t as MeteorParameters).Damage = 0;
				(t as MeteorParameters).Velocity *= 0.6f;
			}
			return t;
		}).ToList();
		
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
