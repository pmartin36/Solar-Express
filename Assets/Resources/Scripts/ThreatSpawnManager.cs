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
			new LaserShipParameters(0, Colors.Blue, 4.5f, 135, 8, 7),
			new MeteorParameters(3, Colors.Blue, Utils.AngleToVector(-70)*7, -70, 3.5f),
			new MeteorParameters(11, Colors.Blue, Utils.AngleToVector(-70)*7, -70, 3f),
			new PointBeamParameters(12, Colors.Green, 10, Vector3.one * 2.5f),
			new MeteorParameters(15, Colors.Blue, Utils.AngleToVector(-70)*7, -70, 3.5f),
			new MeteorParameters(20, Colors.Blue, Utils.AngleToVector(-70)*7, -70, 4.5f),

			new LaserShipParameters(25f, Colors.Red, 4.5f, 45, 6, 7),
			new MeteorParameters(26f, Colors.Yellow, new Vector3(3,7), 70, 2f),
			new MeteorParameters(30f, Colors.Red, new Vector3(2.5f,7), 70, 5f),
			new MeteorParameters(32f, Colors.Red, new Vector3(3.5f,7), 70, 6f),			

			new MeteorParameters(35.5f, Colors.Yellow, new Vector3(0.5f,7), 90, 3.5f),
			new MeteorParameters(35.5f, Colors.Yellow, new Vector3(0,-7), -90, 3.5f),
			new MeteorParameters(35.5f, Colors.Yellow, new Vector3(-0.5f,7), 90, 3.5f),

			new MeteorParameters(42f, Colors.Yellow, new Vector3(-7, 0), 180, 3.5f),
			new MeteorParameters(43f, Colors.Yellow, new Vector3(7, -0.75f), 0, 2f),
			new MeteorParameters(44f, Colors.Yellow, new Vector3(7, 0.75f), 0, 4.5f),

			new LaserShipParameters(42f, Colors.Green, 4.5f, -90, 4, 7),

			new PointBeamParameters(51, Colors.Green, 4, Utils.AngleToVector(35) * 2.5f),
			new PointBeamParameters(56, Colors.Red, 4, Utils.AngleToVector(180) * 2.5f),
			new PointBeamParameters(61, Colors.Green, 4, Utils.AngleToVector(90) * 2.5f),
			new PointBeamParameters(66, Colors.Red, 4, Utils.AngleToVector(-30) * 2.5f),
			new PointBeamParameters(71, Colors.Green, 4, Utils.AngleToVector(0) * 2.5f),
			new PointBeamParameters(76, Colors.Red, 4, Utils.AngleToVector(90) * 2.5f),

			new LevelEnd(90, 700)
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
