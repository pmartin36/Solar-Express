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
			new LaserShipParameters(0, Colors.Yellow, 4f, 90, 3, 3),
			new EMPParameters(4, Colors.Yellow, new Vector2(1f,1.2f)),
			new EMPParameters(4, Colors.Yellow, new Vector2(-1f,1.2f)),

			new EMPParameters(10, Colors.Yellow, new Vector2(1f,1.2f)),
			new EMPParameters(10, Colors.Yellow, new Vector2(-1f,1.2f)),

			new LaserShipParameters(20, Colors.Blue, 3f, -120, 2, 4),
			new LaserShipParameters(22, Colors.Blue, 3f, 60, 2, 4),
			new LaserShipParameters(24, Colors.Red, 4.5f, 45, 2, 6),

			new EMPParameters(24, Colors.Blue, Utils.AngleToVector(-30)*1.5f),
			new EMPParameters(24, Colors.Red, Utils.AngleToVector(150)*1.5f),

			new EMPParameters(26, Colors.Red, Utils.AngleToVector(0)*1.5f),
			new EMPParameters(26, Colors.Blue, Utils.AngleToVector(180)*1.5f),

			new LaserShipParameters(48, Colors.Blue, 3f, 30, 2, 2),
			new LaserShipParameters(50, Colors.Green, 4f, 60, 2, 8),
			new LaserShipParameters(55, Colors.Yellow, 3.5f, -90, 1, 6),

			new EMPParameters(55, Colors.Yellow, Utils.AngleToVector(-115)*1.5f),
			new EMPParameters(55, Colors.Yellow, Utils.AngleToVector(-75)*1.5f),

			new LaserShipParameters(73, Colors.Blue, 3f, 90, 2, 6),

			new EMPParameters(80, Colors.Blue, Utils.AngleToVector(-165)*1.5f),
			new EMPParameters(80, Colors.Blue, Utils.AngleToVector(-75)*1.5f),
			new EMPParameters(80, Colors.Blue, Utils.AngleToVector(105)*1.5f),

			new EMPParameters(82, Colors.Blue, Utils.AngleToVector(0)*1.5f),
			new EMPParameters(82, Colors.Blue, Utils.AngleToVector(90)*1.5f),
			new EMPParameters(82, Colors.Blue, Utils.AngleToVector(180)*1.5f),

			new EMPParameters(84, Colors.Blue, Utils.AngleToVector(-15)*1.5f),
			new EMPParameters(84, Colors.Blue, Utils.AngleToVector(75)*1.5f),
			new EMPParameters(84, Colors.Blue, Utils.AngleToVector(-105)*1.5f),

			new LevelEnd(95, 1000)
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
