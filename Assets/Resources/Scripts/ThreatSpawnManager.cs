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
			new LaserShipParameters(0, Colors.Red, 3.5f, 135, 3, 3),
			new PointBeamParameters(3f, Colors.Red, 15, Utils.AngleToVector(-45)*4),
			new EMPParameters(6f, Colors.Red, new Vector2(0.8f,-1.2f)),
			new EMPParameters(9f, Colors.Red, new Vector2(1.2f,-0.8f)),

			new LaserShipParameters(20, Colors.Green, 3f, 20, 2, 2),
			new LaserShipParameters(20, Colors.Green, 3f, 160, 2, 3),
			new PointBeamParameters(22, Colors.Blue, 4, Utils.AngleToVector(-90)*3),
			
			new EMPParameters(26.5f, Colors.Blue, new Vector2(-0.8f,-1.2f)),
			new EMPParameters(26.5f, Colors.Blue, new Vector2(0.8f,-1.2f)),
			new EMPParameters(27.5f, Colors.Blue, new Vector2(-1f,1f)),
			new LaserShipParameters(29, Colors.Blue, 3.5f, 90, 3, 3),

			new LaserShipParameters(33, Colors.Yellow, 4f, -70, 3, 3),
			new EMPParameters(35, Colors.Green, new Vector2(1.2f, 0.8f)),

			new LaserShipParameters(37, Colors.Green, 3f, 180, 4, 3),
			new PointBeamParameters(51, Colors.Yellow, 14, Utils.AngleToVector(-90)*3),
			new LaserShipParameters(52, Colors.Green, 3f, 0, 4, 2.25f),

			new LaserShipParameters(66.5f, Colors.Green, 3f, 160, 1, 2.5f),
			new EMPParameters(68.25f, Colors.Green, new Vector2(-0.8f, 1.2f)),
			new EMPParameters(68.5f, Colors.Green, new Vector2(1f, 1f)),
			new PointBeamParameters(69, Colors.Green, 4, Utils.AngleToVector(-110)*3),

			new LevelEnd(82)
		};

		float startTime = 53;
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
