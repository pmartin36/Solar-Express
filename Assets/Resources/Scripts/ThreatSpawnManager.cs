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
			new PointBeamParameters(0, Colors.Green, 5, Vector3.one * 3f),
			new MeteorParameters(4, Colors.Green, Utils.AngleToVector(-70)*7, -70, 4.2f),
			new PointBeamParameters(5, Colors.Green, 6, Vector3.one * -3f),

			new OrbitingEnemyParameters(6, Colors.Green, Utils.AngleToVector(180)*7, moveSpeed: 2f, angle: 170),
			new PointBeamParameters(10, Colors.Yellow, 14, new Vector3(-3,3)),

			new LaserShipParameters(14, Colors.Green, 3.5f, 90, 3, 7),
			new LaserShipParameters(14, Colors.Yellow, 3.5f, -90, 3, 6),

			new EMPParameters(14, Colors.Yellow, new Vector2(-1.2f,-0.4f)),
			new EMPParameters(16, Colors.Green, new Vector2(1.2f,0.4f)),
			new EMPParameters(18, Colors.Yellow, new Vector2(0.4f,1.2f)),

			new MeteorParameters(27, Colors.Blue, Utils.AngleToVector(55)*7, 45, 6f),
			new MeteorParameters(27, Colors.Red, Utils.AngleToVector(125)*7, 135, 6f),

			new MeteorParameters(28, Colors.Blue, Utils.AngleToVector(60)*7, 75, 3f, points: 0),
			new MeteorParameters(28, Colors.Blue, Utils.AngleToVector(30)*7, 30, 4f),
			new MeteorParameters(28, Colors.Blue, Utils.AngleToVector(55)*7, 45, 3.5f, points: 0),
			new MeteorParameters(28, Colors.Blue, Utils.AngleToVector(40)*7, 45, 2.5f),

			new MeteorParameters(30, Colors.Red, Utils.AngleToVector(140)*7, 135, 3f),
			new MeteorParameters(30, Colors.Red, Utils.AngleToVector(125)*7, 135, 4f, points: 0),
			new MeteorParameters(30, Colors.Red, Utils.AngleToVector(150)*7, 150, 3.5f),
			new MeteorParameters(30, Colors.Red, Utils.AngleToVector(120)*7, 105, 2.5f, points: 0),

			new LaserShipParameters(33, Colors.Blue, 3f, 0, 2, 3.5f),
			new LaserShipParameters(33, Colors.Red, 3f, 180, 2, 7),
			new MeteorParameters(41, Colors.Yellow, Utils.AngleToVector(20)*7, 20, 4f),

			new OrbitingEnemyParameters(48, Colors.Red, Utils.AngleToVector(90)*7, moveSpeed: 3f, angle: 80),

			new EMPParameters(54, Colors.Yellow, new Vector2(-1f,1f)),

			new EMPParameters(56, Colors.Red, new Vector2(-1.5f,0f)),
			new EMPParameters(56, Colors.Green, new Vector2(-1f,-1f)),

			new EMPParameters(58, Colors.Blue, new Vector2(0f,-1.5f)),
			new EMPParameters(58, Colors.Blue, new Vector2(1f,-1f)),

			new EMPParameters(62, Colors.Green, new Vector2(1.5f,0f)),
			new EMPParameters(62, Colors.Red, new Vector2(1f,1f)),

			new EMPParameters(64, Colors.Yellow, new Vector2(0f,1.5f)),

			new MeteorParameters(64, Colors.Yellow, Utils.AngleToVector(135)*7, 135, 3f),
			new MeteorParameters(65, Colors.Red, Utils.AngleToVector(180)*7, 180, 3f),
			new MeteorParameters(66, Colors.Green, Utils.AngleToVector(-135)*7, -135, 3f),
			new MeteorParameters(67, Colors.Blue, Utils.AngleToVector(-90)*7, -90, 3f),

			new MeteorParameters(69, Colors.Blue, Utils.AngleToVector(-45)*7, -45, 3f),
			new MeteorParameters(70, Colors.Green, Utils.AngleToVector(0)*7, 0, 3f),
			new MeteorParameters(71, Colors.Red, Utils.AngleToVector(45)*7, 45, 3f),
			new MeteorParameters(72, Colors.Yellow, Utils.AngleToVector(90)*7, 90, 3f),

			new OrbitingEnemyParameters(76, Colors.Red, Utils.AngleToVector(90)*7, moveSpeed: 2f, angle: 80),
			new OrbitingEnemyParameters(76, Colors.Green, Utils.AngleToVector(180)*7, moveSpeed: 2f, angle: 170),

			new OrbitingEnemyParameters(83, Colors.Red, Utils.AngleToVector(90)*7, moveSpeed: 2f, angle: 80),
			new OrbitingEnemyParameters(83, Colors.Blue, Utils.AngleToVector(-90)*7, moveSpeed: 2f, angle: -100),

			new LevelEnd(89,1000)
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
